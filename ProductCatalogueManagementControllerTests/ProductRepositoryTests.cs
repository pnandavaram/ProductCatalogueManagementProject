using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Repository;
using ProductCatalogueManagement.Repository.Repository;

public class ProductRepositoryTests
{
    private SqliteConnection _connection;

    private DbContextOptions<AppDbContext> _options;

    private Mock<ILogger<ProductRepository>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AppDbContext(_options);

        context.Database.EnsureCreated();

        _loggerMock = new Mock<ILogger<ProductRepository>>();
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
    }

    private AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    [Test]
    public async Task AddNewProduct_ShouldPersistProduct()
    {
        using var context = CreateContext();

        var repo = new ProductRepository(context, _loggerMock.Object);

        var Id = Guid.NewGuid();

        var product = new ProductData { Id = Id, Name = "Book", Description = "Horror & Fictional Books" };

        var result = await repo.AddNewProduct(product);

        using var verifyContext = CreateContext();

        var result1 = await verifyContext.Products.FirstOrDefaultAsync();

        Assert.That(result1.Name , Is.EqualTo("Book"));
        Assert.That(result1.Id, Is.EqualTo(Id));
    }

    [Test]
    public async Task GetAllProductInfo_ShouldReturnAllProducts()
    {
        using (var context = CreateContext())
        {
            context.Products.AddRange(
                new ProductData { Id = Guid.NewGuid(), Name = "Book", Description = "Horror Books" },
                new ProductData { Id = Guid.NewGuid(), Name = "Pens", Description = "Blue Pens" }
            );
            await context.SaveChangesAsync();
        }

        using var context2 = CreateContext();

        var repo = new ProductRepository(context2, _loggerMock.Object);

        var result = await repo.GetAllProductInfo();

        Assert.That(result, !Is.Null);
    }

    [Test]
    public async Task GetAllProductInfo_ShouldReturnEmptyList_WhenNoData()
    {
        using var context = CreateContext();

        var repo = new ProductRepository(context, _loggerMock.Object);

        var result = await repo.GetAllProductInfo();

        Assert.That(result, !Is.Null);
        Assert.That(result.Count, Is.Zero);
    }

    [Test]
    public async Task GetProductInfoById_ShouldReturnProduct_WhenExists()
    {
        var id = Guid.NewGuid();

        using (var context = CreateContext())
        {
            context.Products.Add(new ProductData
            {
                Id = id,
                Name = "Books",
                Description = "Fiction & Horror books"
            });
            await context.SaveChangesAsync();
        }

        using var context2 = CreateContext();

        var repo = new ProductRepository(context2, _loggerMock.Object);

        var result = await repo.GetProductInfoById(id);

        Assert.That(result, !Is.Null);
        Assert.That(result.Description, !Is.Null);
        Assert.That(result.Description, Is.EqualTo("Fiction & Horror books"));
    }

    [Test]
    public async Task GetProductInfoById_ShouldReturnNull_WhenNotFound()
    {
        using var context = CreateContext();

        var repo = new ProductRepository(context, _loggerMock.Object);

        var result = await repo.GetProductInfoById(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }
}