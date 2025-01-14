using MvcLaptop.Models;

namespace MvcLaptop.Services;

public interface ILaptopService
{
    Task<IEnumerable<LaptopViewModel>> GetLaptops(string? sortOrder = null,string? currentFilter = null,string? searchString = null,int? pageNumber = null,int pageSize = 3);
    Task<LaptopViewModel> GetLaptopById(int id);
    Task<Product> Create(LaptopRequest request,IFormFile mainImage);
    Task<bool> Update(int id, LaptopViewModel laptop, IFormFile? MainImage);
    Task<bool> Delete(int id);
    Task<IEnumerable<Category>> GetCategories();
    bool LaptopExists(int id);
}
public interface ICartService
{
    Dictionary<int, int> GetCartFromSession();
    void SaveCartToSession(Dictionary<int, int> cartItems);
    void AddToCart(int id, int quantity = 1);
    void RemoveFromCart(int id);
    decimal CalculateTotalPrice(Dictionary<int, int> cartItems);
    Task<IEnumerable<dynamic>> GetCartProductsAsync();
    Task<bool> CheckProductStockAsync(Dictionary<int, int> quantities);
    Task ProcessOrderAsync(Order order, Dictionary<int, int> cartItems, string userName, string email);
    Task<bool> ProcessCheckoutAsync(Order order, string userName, string email, int userId);
}
