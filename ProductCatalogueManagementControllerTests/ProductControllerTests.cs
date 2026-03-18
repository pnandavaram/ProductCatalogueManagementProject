using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalogueManagement.API.Controllers;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Models.Enums;
using ProductCatalogueManagement.Services.Interfaces;
using NUnit.Framework;

namespace ProductCatalogueManagementControllerTests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;

        private readonly ProductsController _productsController;

        List<ProductInfoResponse> allProducts = new List<ProductInfoResponse>
                                                {
                                                    new ProductInfoResponse { Id = Guid.NewGuid(), Name = "Shelock Holmes Novel", Description = "A thriller book", Price = 22.55m, Stock = 3, Status = "OK" },
                                                    new ProductInfoResponse { Id = Guid.NewGuid(), Name = "Harry Potter Novel", Description = "A fictional fantasy book", Price = 32.55m, Stock = 1, Status = "OK" },
                                                    new ProductInfoResponse { Id = Guid.NewGuid(), Name = "Shelock Holmes Novel", Description = "A thriller book", Price = null, Stock = null, Status = "Inventory Data Unavailable" },
                                                    new ProductInfoResponse { Id = Guid.NewGuid(), Name = "Blue Lagoon Novel", Description = "A horror book", Price = 10.00m, Stock = 17, Status = "OK" },
                                                };
        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _productsController = new ProductsController(_mockProductService.Object);
        }        

        [Test]
        public async Task Get_ReturnsOk_WhenProductsExist()
        {
            _mockProductService.Setup(s => s.GetAllProductInfo())
                        .ReturnsAsync(allProducts);

            var result = await _productsController.Get();

            var getResult = (OkObjectResult)result;

            Assert.That(getResult.Value, Is.EqualTo(allProducts));
            Assert.That(getResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenNoProducts()
        {
            List<ProductInfoResponse> returnNoProducts = new List<ProductInfoResponse>();

            _mockProductService.Setup(s => s.GetAllProductInfo())
                        .ReturnsAsync(returnNoProducts);

            var result = await _productsController.Get();

            var getResult = result as ObjectResult;

            Assert.That(getResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenProductFound()
        {
            var returnSingleItem = allProducts.Where(p => p.Name.Contains("Shelock")).FirstOrDefault();

            _mockProductService.Setup(s => s.GetProductInfoById(It.IsAny<Guid>()))
                        .ReturnsAsync((returnSingleItem, ProductStatus.ProductFound));

            var result = await _productsController.Get(Guid.NewGuid());

            var getResult = (OkObjectResult)result;

            Assert.That(getResult.Value, Is.EqualTo(returnSingleItem));
            Assert.That(getResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetById_Returns200_WhenInventoryNotFound()
        {
            var returnSingleItem = allProducts.Where(p => p.Status.Equals("Inventory Data Unavailable")).FirstOrDefault();

            _mockProductService.Setup(s => s.GetProductInfoById(It.IsAny<Guid>()))
                        .ReturnsAsync((returnSingleItem, ProductStatus.InventoryNotFound));

            var result = await _productsController.Get(Guid.NewGuid());
           
            var getResult = (ObjectResult)result;

            Assert.That(getResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenProductNotFound()
        {
            _mockProductService.Setup(s => s.GetProductInfoById(It.IsAny<Guid>()))
                        .ReturnsAsync((null, ProductStatus.ProductNotFound));

            var result = await _productsController.Get(Guid.NewGuid());

            var getResult = (ObjectResult)result;

            Assert.That(getResult.StatusCode, Is.EqualTo(404));
        }
        
        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid()
        {
            var product = new ProductData();

            _productsController.ModelState.AddModelError("Name", "Required");

            var result = await _productsController.Create(product);

            var getResult = (ObjectResult)result;

            Assert.That(getResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Create_Returns500_WhenServiceFails()
        {
            var returnNewProduct = new ProductData { Name = "The Crown", Description = "A story of a Great Britain Royal Family" };

            _mockProductService.Setup(s => s.AddNewProduct(It.IsAny<ProductData>()))
                        .ReturnsAsync((returnNewProduct, ProductStatus.Failed));

            var result = await _productsController.Create(returnNewProduct);

            var objResult = result as ObjectResult;

            Assert.That(objResult.StatusCode, Is.EqualTo(500));
        }

    }
}