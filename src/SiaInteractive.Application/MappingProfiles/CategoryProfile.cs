using AutoMapper;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Application.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name!.Trim()));

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name!.Trim()));
        }
    }
}
