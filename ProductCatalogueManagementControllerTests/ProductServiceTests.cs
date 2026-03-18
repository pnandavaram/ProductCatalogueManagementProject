using Moq;
using ProductCatalogueManagement.Services.Services;
using ProductCatalogueManagement.Repository.Interfaces;
using ProductCatalogueManagement.Services.Interfaces;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Models.Enums;

namespace ProductCatalogueManagementControllerTests
{
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _repoMock;
        private Mock<IInventoryService> _inventoryMock;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IProductRepository>();
            _inventoryMock = new Mock<IInventoryService>();
            _productService = new ProductService(_repoMock.Object, _inventoryMock.Object);
        }

        [Test]
        public async Task AddNewProduct_ShouldReturnSuccess_WhenRepositorySucceeds()
        {
            var inputProduct = new ProductData { Name = "The Crown", Description = "A story of a Great Britain Royal Family" };

            _repoMock.Setup(r => r.AddNewProduct(It.IsAny<ProductData>())).ReturnsAsync(inputProduct);

            var (result, status) = await _productService.AddNewProduct(inputProduct);
                
            Assert.That(status, Is.EqualTo(ProductStatus.SuccessfullyCreated));
            Assert.That(result.Name, Is.EqualTo("The Crown"));
        }

        [Test]
        public async Task AddNewProduct_ShouldReturnFailed_WhenRepositoryThrows()
        {
            var inputProduct = new ProductData();

            _repoMock.Setup(r => r.AddNewProduct(It.IsAny<ProductData>())).ThrowsAsync(new Exception());

            var (result, status) = await _productService.AddNewProduct(inputProduct);

            Assert.That(result, Is.EqualTo(null));
            Assert.That(status, Is.EqualTo(ProductStatus.Failed));
        }

        [Test]
        public async Task GetAllProductInfo_ShouldReturnProducts_WhenInventoryAvailable()
        {
            var products = new List<ProductData>
            {
                new ProductData { Id = Guid.NewGuid(), Name = "Book", Description = "Horror & Fictional Books" },
                new ProductData { Id = Guid.NewGuid(), Name = "Pens", Description = "Blue & Black Pens" },
                new ProductData { Id = Guid.NewGuid(), Name = "Notepad", Description = "Ruled Notepads" }
            };

            _repoMock.Setup(r => r.GetAllProductInfo()).ReturnsAsync(products);

            _inventoryMock.Setup(i => i.GetInventoryAsync(It.IsAny<Guid>())).ReturnsAsync((110.90m, 5, InventoryAPIStatus.Available));

            var result = await _productService.GetAllProductInfo();

            Assert.That(result[0].Name, Is.EqualTo("Book"));
            Assert.That(result[0].Price, Is.EqualTo(110.90m));
            Assert.That(result[0].Status, Is.EqualTo("OK"));
        }

        [Test]
        public async Task GetAllProductInfo_ShouldReturnUnavailableStatus_WhenInventoryUnavailable()
        {
            var product = new List<ProductData>
            {
                new ProductData { Id = Guid.NewGuid(), Name = "Book", Description = "Horror & Fictional Books" }
            };

            _repoMock.Setup(r => r.GetAllProductInfo()).ReturnsAsync(product);

            _inventoryMock.Setup(i => i.GetInventoryAsync(It.IsAny<Guid>())).ReturnsAsync((null, null, InventoryAPIStatus.Unavailable));

            var result = await _productService.GetAllProductInfo();

            Assert.That(result[0].Name, Is.EqualTo("Book"));
            Assert.That(result[0].Price, Is.EqualTo(null));
            Assert.That(result[0].Status, Is.EqualTo("Inventory Data Unavailable"));
        }

        [Test]
        public async Task GetProductInfoById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetProductInfoById(id)).ReturnsAsync((ProductData)null);

            var (result, status) = await _productService.GetProductInfoById(id);

            Assert.That(result, Is.EqualTo(null));
            Assert.That(status, Is.EqualTo(ProductStatus.ProductNotFound));
        }

        [Test]
        public async Task GetProductInfoById_ShouldReturnInventoryUnavailable()
        {
            var id = Guid.NewGuid();

            var product = new ProductData { Id = id, Name = "Book", Description = "Horror & Fictional Books" };

            _repoMock.Setup(r => r.GetProductInfoById(id)).ReturnsAsync(product);

            _inventoryMock.Setup(i => i.GetInventoryAsync(id)).ReturnsAsync((null, null, InventoryAPIStatus.Unavailable));

            var (result, status) = await _productService.GetProductInfoById(id);

            Assert.That(result.Name, Is.EqualTo("Book"));
            Assert.That(result.Price, Is.EqualTo(null));
            Assert.That(result.Stock, Is.EqualTo(null));
            Assert.That(result.Status, Is.EqualTo("Inventory Data Unavailable"));
        }

        [Test]
        public async Task GetProductInfoById_ShouldReturnProduct_WhenInventoryAvailable()
        {
            var id = Guid.NewGuid();

            var product = new ProductData { Id = id, Name = "Book", Description = "Horror & Fictional Books" };

            _repoMock.Setup(r => r.GetProductInfoById(id)).ReturnsAsync(product);

            _inventoryMock.Setup(i => i.GetInventoryAsync(id)).ReturnsAsync((200.90m, 10, InventoryAPIStatus.Available));

            var (result, status) = await _productService.GetProductInfoById(id);

            Assert.That(result.Name, Is.EqualTo("Book"));
            Assert.That(result.Price, Is.EqualTo(200.90m));
            Assert.That(result.Stock, Is.EqualTo(10));
            Assert.That(result.Status, Is.EqualTo("OK"));
        }
    }
}