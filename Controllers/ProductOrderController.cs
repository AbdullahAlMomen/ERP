using Azure.Core;
using ERP.Entities;
using ERP.Models;
using ERP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductOrderController : ControllerBase
    {
        private readonly IProductOrderService _productOrderService;
        public ProductOrderController(IProductOrderService productOrderService)
        {
               _productOrderService = productOrderService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                ProductId = request.ProductId,
                CustomerName = request.CustomerName,
                Quantity = request.Quantity,
                OrderDate = request.OrderDate,
            };
            return Ok(await _productOrderService.CreateOrder(order));
        }

        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails()
        {
            
            return Ok(await _productOrderService.GetOrderDetails());
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)  
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice,
                Stock = request.Stock,
            };

            var result = await _productOrderService.CreateProduct(product);
            return Ok(result);
        }
    
    }
}
