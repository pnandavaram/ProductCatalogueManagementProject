using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Models.Enums;
using ProductCatalogueManagement.Services.Interfaces;
using System.Net.Http.Json;

namespace ProductCatalogueManagement.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<InventoryService> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public InventoryService(HttpClient httpClient, ILogger<InventoryService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(decimal? price, int? stock, InventoryAPIStatus inventoryApiStatus)> GetInventoryAsync(Guid productId)
        {
            try
            {
                _logger.LogInformation("Calling Inventory API for ProductId: {ProductId}", productId);

                var request = new HttpRequestMessage(HttpMethod.Get, $"inventory/{productId}");

                if (_httpContextAccessor.HttpContext?.Items.TryGetValue("X-Correlation-ID", out var correlationId) == true)
                {
                    request.Headers.Add("X-Correlation-ID", correlationId.ToString());
                }

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode.Equals(500))
                {
                    _logger.LogWarning("Inventory API failed with status: {StatusCode}", response.StatusCode);

                    return (null, null, InventoryAPIStatus.Unavailable);
                }
                            
                var data = await response.Content.ReadFromJsonAsync<InventoryResponse>();

                return (data?.Price, data?.Stock, InventoryAPIStatus.Available);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory API for ProductId: {ProductId}", productId);

                return (null, null, InventoryAPIStatus.Unavailable);
            }
        }
    }
}