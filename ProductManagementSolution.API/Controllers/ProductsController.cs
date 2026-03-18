using Microsoft.AspNetCore.Mvc;
using ProductCatalogueManagement.Models;
using ProductCatalogueManagement.Models.Enums;
using ProductCatalogueManagement.Services.Interfaces;

namespace ProductCatalogueManagement.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var productInfo = await _service.GetAllProductInfo();

            if (productInfo is null)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
            else if (productInfo.Count == 0)
            {
                return NotFound("Unable to find products");
            }
            else
            {
                return Ok(productInfo);
            }      
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var (productInfo, productStatus) = await _service.GetProductInfoById(id);

            switch (productStatus)
            {
                case ProductStatus.ProductFound:
                     
                    return Ok(productInfo);

                case ProductStatus.InventoryNotFound:

                    return StatusCode(200, "Unable to find inventory details for the product " + productInfo);

                case ProductStatus.ProductNotFound:

                    return NotFound("Unable to find the product with Id - " + id);              

                default:
                    return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductData productInfo)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid info provided. Please check the input data.");
            }

            var (productInfo1, productStatus) = await _service.AddNewProduct(productInfo);

            switch(productStatus)
            {
                case ProductStatus.SuccessfullyCreated:

                    return Ok(productInfo);

                case ProductStatus.Failed:

                    return StatusCode(500, "An error occurred while saving the product info. Please try again later.");

                default:
                    return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}