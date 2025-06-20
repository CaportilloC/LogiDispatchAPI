using Application.DTOs.Orders;
using Application.DTOs.Orders.Customers;
using Application.DTOs.Orders.Products;
using Application.DTOs.Shared;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Update;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            // Commands -> Request DTOs
            CreateMap<Product, ProductResponse>();
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CreateOrderCommand, CreateOrderRequest>();
            CreateMap<UpdateOrderCommand, UpdateOrderRequest>();
            CreateMap<DeleteOrderCommand, EntityIdDto>();

            // Request DTO -> Entity (para Create)
            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));

            // Request DTO -> Entity (para Update)
            CreateMap<UpdateOrderRequest, Order>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

            // DTO -> Entidad OrderItem (uso interno en Create/Update)
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // Entity -> Response DTO
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.Phone ?? string.Empty))
                .ForMember(dest => dest.OriginLatitude, opt => opt.MapFrom(src => (double)src.OriginLatitude))
                .ForMember(dest => dest.OriginLongitude, opt => opt.MapFrom(src => (double)src.OriginLongitude))
                .ForMember(dest => dest.DestinationLatitude, opt => opt.MapFrom(src => (double)src.DestinationLatitude))
                .ForMember(dest => dest.DestinationLongitude, opt => opt.MapFrom(src => (double)src.DestinationLongitude))
                .ForMember(dest => dest.DistanceKm, opt => opt.MapFrom(src => (double)src.DistanceKm))
                .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => src.ShippingCostUSD))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}
