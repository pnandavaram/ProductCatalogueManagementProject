using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Repository.Interfaces;

namespace ProductCatalogueManagement.Repository.Repository
{
    public class ProductRepository:IProductRepository
    {
        private readonly AppDbContext _context;

        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger) 
        { 
            _context = context;
            _logger = logger;
        }

        public async Task<ProductData> AddNewProduct(ProductData product)
        {
            try
            {
                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product added successfully: {ProductId}", product.Id);

                return product;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error adding product: {ProductName}", product.Name);

                throw new ApplicationException("Error occured while adding a new product.");
            }          
        }

        public async Task<List<ProductData>> GetAllProductInfo()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                _logger.LogInformation("Retrieved {ProductCount} products.", products.Count);

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product information.");

                throw new ApplicationException("Error occured while retrieving the products.");
            }
        }

        public async Task<ProductData> GetProductInfoById(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving product information for ID: {ProductId}", id);

                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product information.");

                throw new ApplicationException("Error occured while retrieving the product.");
            }
        }
    }
}