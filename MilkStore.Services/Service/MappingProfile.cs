using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using MilkStore.ModelViews.OrderDetailsModelView;

namespace MilkStore.Services.Service
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Products, ProductsModel>();
            CreateMap<ProductsModel, Products>();

            CreateMap<UserModelView, ApplicationUser>()
            .ForMember(dest => dest.Points, opt => opt.MapFrom(src => 0)) // Set Points to 0
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

            CreateMap<UserModelView, ApplicationUser>()
            .ForMember(dest => dest.LastUpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.Points, opt => opt.Ignore()); // Nếu không muốn ánh xạ Points

            CreateMap<OrderModelView, Order>();            
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderDetailss, opt => opt.MapFrom(src => src.OrderDetailss));

            CreateMap<OrderDetailsModelView, OrderDetails>();
            CreateMap<OrderDetails, OrderDetailResponseDTO>()
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()); //

            // Ánh xạ từ OrderModelView sang Order, nhưng chỉ cập nhật các thuộc tính thay đổi
            CreateMap<OrderModelView, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Chỉ ánh xạ khi giá trị mới khác null
        }
    }
}
