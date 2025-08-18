using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.Category;

namespace ExpenseTrackler.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Expenses, opt => opt.Ignore());

        // Update -> Entity
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
            .ForMember(dest => dest.Expenses, opt => opt.Ignore());

        // required to populate an edit form with existing user data
        CreateMap<Category, UpdateCategoryDto>();

        // Entity -> Read DTO
        CreateMap<Category, CategoryDto>();
    }
    
}