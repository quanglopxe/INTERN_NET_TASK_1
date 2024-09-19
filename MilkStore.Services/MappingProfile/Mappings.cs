using AutoMapper;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.ProductsModelViews;

namespace MilkStore.Services.MappingProfile
{
    public class Mappings: Profile
    {
        public Mappings()
        {
            CreateMap<Products, ProductsModel>();
            CreateMap<ProductsModel, Products>();
        }
    }
}
