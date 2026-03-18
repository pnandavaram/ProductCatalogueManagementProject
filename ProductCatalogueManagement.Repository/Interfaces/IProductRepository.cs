using ProductCatalogueManagement.Models;

namespace ProductCatalogueManagement.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductData>> GetAllProductInfo();

        Task<ProductData> GetProductInfoById(Guid id);

        Task<ProductData> AddNewProduct(ProductData product);
    }
}