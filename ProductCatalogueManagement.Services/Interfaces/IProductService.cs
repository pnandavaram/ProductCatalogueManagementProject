using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Models.Enums;

namespace ProductCatalogueManagement.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductInfoResponse>> GetAllProductInfo();

        Task<(ProductInfoResponse, ProductStatus)> GetProductInfoById(Guid id);

        Task<(ProductData, ProductStatus)> AddNewProduct(ProductData productInfo);
    }
}