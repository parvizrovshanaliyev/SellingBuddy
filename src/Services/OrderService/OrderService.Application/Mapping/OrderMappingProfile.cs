using System.Linq;
using OrderService.Application.Features.Order.Commands;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDetailResponse>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Address.ZipCode))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus.Name))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.OrderItems.Sum(x => x.Units * x.UnitPrice)))
            .ReverseMap();

        CreateMap<Order, CreateOrderCommand>()
            .ReverseMap();
        
        CreateMap<OrderItem, OrderItemDto>()
            .ReverseMap();
        
        CreateMap<OrderItemDto, OrderItemModel>()
            .ReverseMap();
    }
}