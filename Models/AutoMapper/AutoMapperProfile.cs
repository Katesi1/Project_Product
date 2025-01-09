using AutoMapper;
namespace MvcLaptop.Models.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, LaptopViewModel>()
        .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
            src.ProductImages!.FirstOrDefault(img => img.IsMainImage)!.ImageUrl ?? "/images/default.jpg"))
        .ForMember(dest => dest.Name_Category, opt => opt.MapFrom(src => 
            src.Category != null ? src.Category.Name_Category : "Unknown")).ReverseMap();
        CreateMap<LaptopRequest, Product>();
    }
}