using Microsoft.EntityFrameworkCore;
using ProductCatalogueManagement.Models;

namespace ProductCatalogueManagement.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<ProductData> Products { get; set; }
    }
}