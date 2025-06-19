using Application.Attributes.Services;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Specifications.Orders;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.OrderService
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShippingCalculatorService _shippingCalculatorService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IShippingCalculatorService shippingCalculatorService,
            ILogger<OrderService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shippingCalculatorService = shippingCalculatorService;
            _logger = logger;
            
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var spec = new OrderWithOrderItemsSpecification();
            var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);

            if (orders == null || !orders.Any())
            {
                _logger.LogWarning("No se encontraron órdenes registradas.");
                return new List<OrderResponse>();
            }

            var responses = _mapper.Map<List<OrderResponse>>(orders);

            _logger.LogInformation("Se recuperaron {Count} órdenes", responses.Count);
            return responses;
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id)
        {
            var spec = new OrderWithOrderItemsByIdSpecification(id);
            var order = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(spec);

            if (order == null)
            {
                _logger.LogWarning("Orden con ID {OrderId} no encontrada.", id);
                return null;
            }

            var response = _mapper.Map<OrderResponse>(order);
            _logger.LogInformation("Se recuperó la orden con ID {OrderId}", id);

            return response;
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
        {
            // Verificar si el cliente existe (puede delegarse al handler si prefieres)
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new Exception("Cliente no encontrado.");

            // Calcular distancia y costo de envío (usando servicio async)
            var (distanceKm, shippingCost) = await _shippingCalculatorService.CalculateShippingAsync(
                (double)request.OriginLatitude,
                (double)request.OriginLongitude,
                (double)request.DestinationLatitude,
                (double)request.DestinationLongitude
            );

            // Crear la orden
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OriginLatitude = request.OriginLatitude,
                OriginLongitude = request.OriginLongitude,
                DestinationLatitude = request.DestinationLatitude,
                DestinationLongitude = request.DestinationLongitude,
                DistanceKm = (decimal)distanceKm,
                ShippingCostUSD = shippingCost,
                CreatedAt = DateTime.UtcNow
            };

            // Agregar los ítems de la orden
            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Producto con ID {item.ProductId} no encontrado.");

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            // Guardar en base de datos
            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.Complete();

            _logger.LogInformation("Orden {OrderId} creada exitosamente", order.Id);

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> UpdateAsync(UpdateOrderRequest request)
        {
            // Buscar la orden existente
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.Id);
            if (order == null || order.DeletedAt != null)
                throw new Exception("Orden no encontrada o fue eliminada.");

            // Verificar si el nuevo cliente existe
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new Exception("Cliente no encontrado.");

            // Calcular nueva distancia y costo
            var (distanceKm, shippingCost) = await _shippingCalculatorService.CalculateShippingAsync(
                (double)request.OriginLatitude,
                (double)request.OriginLongitude,
                (double)request.DestinationLatitude,
                (double)request.DestinationLongitude
            );

            // Validar distancia permitida
            if (distanceKm < 1 || distanceKm > 1000)
                throw new Exception("La distancia calculada está fuera del rango permitido (1–1000 km).");

            // Actualizar datos de la orden
            order.CustomerId = request.CustomerId;
            order.OriginLatitude = request.OriginLatitude;
            order.OriginLongitude = request.OriginLongitude;
            order.DestinationLatitude = request.DestinationLatitude;
            order.DestinationLongitude = request.DestinationLongitude;
            order.DistanceKm = (decimal)distanceKm;
            order.ShippingCostUSD = shippingCost;
            order.UpdatedAt = DateTime.UtcNow;

            // Limpiar ítems actuales
            order.OrderItems.Clear();

            // Agregar los nuevos ítems
            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Producto con ID {item.ProductId} no encontrado.");

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            // Persistir cambios
            await _unitOfWork.Repository<Order>().UpdateAsync(order);
            await _unitOfWork.Complete();

            _logger.LogInformation("Orden {OrderId} actualizada correctamente", order.Id);

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);

            if (order == null || order.DeletedAt != null)
            {
                _logger.LogWarning("No se puede eliminar: orden con ID {OrderId} no encontrada o ya eliminada.", id);
                return false;
            }

            order.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Order>().UpdateAsync(order);
            await _unitOfWork.Complete();

            _logger.LogInformation("Orden {OrderId} eliminada correctamente (soft delete)", id);
            return true;
        }

        public async Task<List<OrderResponse>> GetDeletedAsync()
        {
            var spec = new OrdersDeletedSpecification();
            var deletedOrders = await _unitOfWork.Repository<Order>().ListAsync(spec);

            if (deletedOrders == null || !deletedOrders.Any())
            {
                _logger.LogWarning("No se encontraron órdenes eliminadas.");
                return new List<OrderResponse>();
            }

            var responses = _mapper.Map<List<OrderResponse>>(deletedOrders);
            _logger.LogInformation("Se recuperaron {Count} órdenes eliminadas", responses.Count);

            return responses;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);

            if (order == null || order.DeletedAt == null)
            {
                _logger.LogWarning("No se puede restaurar: orden con ID {OrderId} no existe o no está eliminada.", id);
                return false;
            }

            order.DeletedAt = null;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Order>().UpdateAsync(order);
            await _unitOfWork.Complete();

            _logger.LogInformation("Orden {OrderId} restaurada exitosamente.", id);
            return true;
        }
    }
}