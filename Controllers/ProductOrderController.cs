using Azure.Core;
using ERP.Entities;
using ERP.Models;
using ERP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpPost("BulkOrderInsert")]
        public async Task<IActionResult> BulkOrderInsert(List<CreateOrderRequest> orders)
        {
            return Ok(await _productOrderService.BulkOrderInsertion(orders));
        }

        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails()
        {
            
            return Ok(await _productOrderService.GetOrderDetails());
        }

        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Admin")]
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

        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _productOrderService.UpdateOrder(requestModel));

        }
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            return Ok(await _productOrderService.DeleteOrder(orderId));
        }

        [HttpGet("GetProductRevenueSummaries")]
        public async Task<IActionResult> GetProductRevenueSummaries()
        {
            return Ok(await _productOrderService.GetProductRevenueSummaries());
        }


        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(decimal threshold)
        {
            return Ok(await _productOrderService.GetProductDetails(threshold));
        }
        [HttpGet("GetTopCustomers")]
        public async Task<IActionResult> GetTopCustomers()
        {
            return Ok(await _productOrderService.GetTopCustomers());
        }

        [HttpGet("GetProductWithNoOrders")]
        public async Task<IActionResult> GetProductWithNoOrder()
        {
            return Ok(await _productOrderService.GetProductWithNoOrder());
        }

    }
}
