using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Category;
using ExpenseTracker.Application.DTOs.Category;


namespace ExpenseTracker.API.Contracts.V1.Common.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryRequestV1, CreateCategoryDto>();
        CreateMap<CategoryDto, CategoryResponseV1>();
        CreateMap<CategorySummaryDto, CategorySummaryResponseV1>();
    }
}

// here we map contract request objects to application DTOs