using AutoMapper;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.MappingProfile
{
    public class Mappings: Profile
    {
        public Mappings()
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
        }
    }
}
