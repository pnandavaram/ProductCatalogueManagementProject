using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Services.Interfaces;
using ProductCatalogueManagement.Repository.Interfaces;
using ProductCatalogueManagement.Models.Enums;

namespace ProductCatalogueManagement.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IInventoryService _inventoryService;

        public ProductService(IProductRepository repo, IInventoryService inventoryService)
        {
            _repo = repo;
            _inventoryService = inventoryService;
        }

        public async Task<(ProductData, ProductStatus)> AddNewProduct(ProductData productInfo)
        {
            try
            {
                var product = new ProductData
                {
                    Id = Guid.NewGuid(),
                    Name = productInfo.Name,
                    Description = productInfo.Description
                };

                await _repo.AddNewProduct(product);

                return (product,ProductStatus.SuccessfullyCreated);
            }
            catch 
            {
                return (null, ProductStatus.Failed);  
            }           
        }

        public async Task<List<ProductInfoResponse>> GetAllProductInfo()
        {
            var allProducts = await _repo.GetAllProductInfo();

            var result = new List<ProductInfoResponse>();

            foreach (var product in allProducts)
            {
                ProductInfoResponse dto = new ProductInfoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                };

                
                var inventory = await _inventoryService.GetInventoryAsync(product.Id);

                if(inventory.inventoryApiStatus.Equals(InventoryAPIStatus.Unavailable))
                {
                    dto.Status = "Inventory Data Unavailable";
                }
                else if (inventory.inventoryApiStatus.Equals(InventoryAPIStatus.Available))
                {
                    dto.Price = inventory.price;
                    dto.Stock = inventory.stock;
                    dto.Status = "OK";
                }                          
                
                result.Add(dto);
            }

            return result;            
        }

        public async Task<(ProductInfoResponse?, ProductStatus)> GetProductInfoById(Guid id)
        {
            ProductData productInfo = null;


            productInfo = await _repo.GetProductInfoById(id);

            if (productInfo == null)
            {
                return (null, ProductStatus.ProductNotFound);
            }

            var (price, stock, status) = await _inventoryService.GetInventoryAsync(id);

            if (status.Equals(InventoryAPIStatus.Unavailable))
            {
                return (new ProductInfoResponse
                {
                    Id = productInfo.Id,
                    Name = productInfo.Name,
                    Description = productInfo.Description,
                    Status = "Inventory Data Unavailable"
                }, ProductStatus.InventoryNotFound);
            }
            else
            {
                return (new ProductInfoResponse
                {
                    Id = productInfo.Id,
                    Name = productInfo.Name,
                    Description = productInfo.Description,
                    Price = price,
                    Stock = stock,
                    Status = "OK"
                }, ProductStatus.ProductFound);
            }
        }
    }
}