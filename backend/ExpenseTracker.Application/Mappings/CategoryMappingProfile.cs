using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.Category;

namespace ExpenseTrackler.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateCategoryDto, Category>();

        // Update -> Entity
        CreateMap<UpdateCategoryDto, Category>();

        // required to populate an edit form with existing user data
        CreateMap<UpdateCategoryDto, Category>();

        // Entity -> Read DTO
        CreateMap<Category, CategoryDto>();
    }
    
}