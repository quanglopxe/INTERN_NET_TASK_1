using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.OrderModelViews;

namespace MilkStore.Services.Service
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderModelView, Order>();
            // Ánh xạ từ Order sang OrderResponseDTO
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderDetailss, opt => opt.MapFrom(src => src.OrderDetailss));

            // Ánh xạ từ OrderDetail sang OrderDetailResponseDTO
            CreateMap<OrderDetails, OrderDetailResponseDTO>();

            // Ánh xạ từ OrderModelView sang Order, nhưng chỉ cập nhật các thuộc tính thay đổi
            CreateMap<OrderModelView, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Chỉ ánh xạ khi giá trị mới khác null
        }
    }
}
