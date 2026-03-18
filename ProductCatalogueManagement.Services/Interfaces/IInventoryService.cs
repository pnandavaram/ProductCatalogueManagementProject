using ProductCatalogueManagement.Models.Enums;

namespace ProductCatalogueManagement.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<(decimal? price, int? stock, InventoryAPIStatus inventoryApiStatus)> GetInventoryAsync(Guid productId);
    }
}