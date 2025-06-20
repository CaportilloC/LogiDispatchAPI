using Application.DTOs.Orders;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class ServiceUserProfile : Profile
    {
        public ServiceUserProfile()
        {
            CreateMap<Order, OrderResponse>();
        }
    }
}
