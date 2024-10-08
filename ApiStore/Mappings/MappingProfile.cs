using ApiStore.DTOs;
using ApiStore.Models;
using AutoMapper;

namespace ApiStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Models -> DTO
            CreateMap<Product, ProductResponse>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<Order, OrderResponse>();
            CreateMap<OrderDetail, OrderDetailResponse>();
            CreateMap<User, UserResponse>();
            CreateMap<Category, CategoryResponse>();

            // DTO -> Models
            CreateMap<ProductRequest, Product>();
            CreateMap<CategoryRequest, Category>();
            CreateMap<OrderRequest, Order>();
            CreateMap<OrderDetailRequest, OrderDetail>();
            CreateMap<UserRequest, User>();
            CreateMap<CategoryResponse, Category>();
        }
    }
}
