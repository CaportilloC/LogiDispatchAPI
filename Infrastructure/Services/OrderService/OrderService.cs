using Application.Attributes.Services;
using Application.Contracts.Persistence.Common;
using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.OrderService
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingCalculatorService _shippingCalculatorService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext context,
            IOrderRepository orderRepository,
            IShippingCalculatorService shippingCalculatorService,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _context = context;
            _orderRepository = orderRepository;
            _shippingCalculatorService = shippingCalculatorService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando consulta de todas las órdenes...");

                var orders = await _orderRepository.GetAllAsync();

                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No se encontraron órdenes registradas.");
                    return new List<OrderResponse>();
                }

                var responses = _mapper.Map<List<OrderResponse>>(orders);

                _logger.LogInformation("Se recuperaron {Count} órdenes", responses.Count);
                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las órdenes");
                throw;
            }
        }

        public async Task<List<OrderResponse>> GetDeletedAsync()
        {
            try
            {
                _logger.LogInformation("Consultando órdenes eliminadas...");

                var orders = await _orderRepository.GetAllAsync();

                var deletedOrders = orders
                    .Where(o => o.DeletedAt != null)
                    .ToList();

                return _mapper.Map<List<OrderResponse>>(deletedOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar órdenes eliminadas");
                throw;
            }
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Consultando orden con ID: {OrderId}", id);

                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                {
                    _logger.LogWarning("Orden con ID {OrderId} no encontrada", id);
                    return null;
                }

                var response = _mapper.Map<OrderResponse>(order);
                _logger.LogInformation("Orden {OrderId} recuperada exitosamente", id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar la orden con ID {OrderId}", id);
                throw;
            }
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Iniciando creación de orden para el cliente {CustomerId}", request.CustomerId);

            try
            {
                var (distanceKm, shippingCost) = await _shippingCalculatorService.CalculateShippingAsync(
                    (double)request.OriginLatitude,
                    (double)request.OriginLongitude,
                    (double)request.DestinationLatitude,
                    (double)request.DestinationLongitude
                );

                var order = _mapper.Map<Order>(request);
                order.DistanceKm = (decimal)distanceKm;
                order.ShippingCostUSD = shippingCost;
                order.Status = OrderStatus.Created;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Orden {OrderId} creada exitosamente", order.Id);

                await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
                foreach (var item in order.OrderItems)
                {
                    await _context.Entry(item).Reference(i => i.Product).LoadAsync();
                }

                return _mapper.Map<OrderResponse>(order);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al crear la orden para el cliente {request.CustomerId}: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request)
        {
            _logger.LogInformation("Iniciando actualización de la orden {OrderId}", id);

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    throw new KeyNotFoundException("Orden no encontrada.");

                if (order.Status != OrderStatus.Created)
                    throw new InvalidOperationException("Solo se pueden modificar órdenes con estado 'Created'.");

                var (distanceKm, shippingCost) = await _shippingCalculatorService.CalculateShippingAsync(
                    (double)request.OriginLatitude,
                    (double)request.OriginLongitude,
                    (double)request.DestinationLatitude,
                    (double)request.DestinationLongitude
                );

                // Mapeo del request a la entidad
                _mapper.Map(request, order);
                order.DistanceKm = (decimal)distanceKm;
                order.ShippingCostUSD = shippingCost;

                // Reemplazar los items
                _context.OrderItems.RemoveRange(order.OrderItems);

                var newItems = request.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();

                await _context.OrderItems.AddRangeAsync(newItems);
                await _context.SaveChangesAsync();

                // Cargar relaciones necesarias
                await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
                foreach (var item in newItems)
                {
                    await _context.Entry(item).Reference(i => i.Product).LoadAsync();
                }

                order.OrderItems = newItems;

                _logger.LogInformation("Orden {OrderId} actualizada correctamente", id);
                return _mapper.Map<OrderResponse>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la orden {OrderId}", id);
                throw new ApplicationException($"Error al actualizar la orden {id}", ex);
            }
        }

        public async Task<OrderResponse> DeleteAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new KeyNotFoundException("Orden no encontrada.");

            if (order.Status != OrderStatus.Created)
                throw new InvalidOperationException("Solo se pueden eliminar órdenes con estado 'Created'.");

            if (order.DeletedAt != null)
                throw new InvalidOperationException("La orden ya fue eliminada.");

            order.DeletedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> RestoreAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new KeyNotFoundException("Orden no encontrada.");

            if (order.DeletedAt == null)
                throw new InvalidOperationException("La orden no está eliminada.");

            order.DeletedAt = null;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponse>(order); // <-- Requiere AutoMapper configurado
        }

    }
}