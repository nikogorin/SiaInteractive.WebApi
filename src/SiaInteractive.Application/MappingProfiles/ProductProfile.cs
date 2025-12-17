using AutoMapper;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image != null ? Convert.ToBase64String(src.Image) : null))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Image) ? null : Convert.FromBase64String(src.Image)))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name!.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Image) ? null : Convert.FromBase64String(src.Image)))
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name!.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Image) ? null : Convert.FromBase64String(src.Image)))
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
