using ERP.Entities;
using ERP.Models;
using ERP.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ERP.Services
{

    public interface IProductOrderService
    {
        public Task<CommonResponse> CreateOrder(Order order);
        public Task<CommonResponse> UpdateOrder(OrderUpdateRequestModel order);
        public Task<CommonResponse> DeleteOrder(int orderId);
        public Task<CommonResponse> GetOrderDetails();
        public Task<CommonResponse> CreateProduct(Product product);
    }

    public class ProdcutOrderService : IProductOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ProdcutOrderService> _logger;

        public ProdcutOrderService(IProductRepository productRepository, IOrderRepository orderRepository, ILogger<ProdcutOrderService> logger)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<CommonResponse> CreateOrder(Order order)
        {
            CommonResponse response = new CommonResponse();
            if (order == null)
            {
                _logger.LogError("Order is Null " + JsonSerializer.Serialize(order));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("Order Can't be null");
                return response;
            }
            Product product = await _productRepository.GetByIdAsync(order.ProductId);
            if (product == null)
            {
                _logger.LogError("Not a Valid Product " + JsonSerializer.Serialize(product));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("Not a Valid Product");
                return response;
            }
            if (product.Stock < order.Quantity)
            {
                _logger.LogError(product.ProductName + " stock less than order Quantity  " + JsonSerializer.Serialize(product));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Message.Add(product.ProductName + " stock less than order Quantity");
                return response;
            }
            try
            {
                await _orderRepository.CreateAsync(order);
                product.Stock -= order.Quantity;
                await _productRepository.UpdateAsync(product);
                response.Code = StatusCodes.Status200OK;
                response.Status = StatusCodes.Status200OK.ToString();
                response.Data = new { ProductName = product.ProductName, OrderQuantity = order.Quantity };
                response.Message.Add("Order is created succesfully");
                _logger.LogInformation("Order is created succesfully ");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Code = StatusCodes.Status422UnprocessableEntity;
                response.Status = StatusCodes.Status422UnprocessableEntity.ToString();
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<CommonResponse> UpdateOrder(OrderUpdateRequestModel order)
        {
            CommonResponse response = new CommonResponse();
            if (order == null)
            {
                _logger.LogError("OrderUpdateRequestModel is Null " + JsonSerializer.Serialize(order));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("OrderUpdateRequestModel Can't be null");
                return response;
            }
            Order orderToUpdate = await _orderRepository.GetByIdAsync(order.orderId);
            if (orderToUpdate == null)
            {
                _logger.LogError("Order not found  " + JsonSerializer.Serialize(order));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("Order not found");
                return response;
            }
            Product product = await _productRepository.GetByIdAsync(orderToUpdate.ProductId);
            if (product == null)
            {
                _logger.LogError("Not a Valid Product " + JsonSerializer.Serialize(product));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("Not a Valid Product");
                return response;
            }
            orderToUpdate.Quantity += order.orderQuantity;
            if (product.Stock < orderToUpdate.Quantity)
            {
                _logger.LogError(product.ProductName + " stock less than order Quantity  " + JsonSerializer.Serialize(product));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Message.Add(product.ProductName + " stock less than order Quantity");
                return response;
            }
            try
            {
                await _orderRepository.UpdateAsync(orderToUpdate);
                product.Stock -= order.orderQuantity;
                await _productRepository.UpdateAsync(product);
                response.Code = StatusCodes.Status200OK;
                response.Status = StatusCodes.Status200OK.ToString();
                response.Data = new { ProductName = product.ProductName, OrderQuantity = order.orderQuantity };
                response.Message.Add("Order is updated succesfully");
                _logger.LogInformation("Order is updated succesfully ");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Code = StatusCodes.Status422UnprocessableEntity;
                response.Status = StatusCodes.Status422UnprocessableEntity.ToString();
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<CommonResponse> DeleteOrder(int orderId)
        {
            CommonResponse response = new CommonResponse();

            Order orderToDelete = await _orderRepository.GetByIdAsync(orderId);
            if (orderToDelete == null)
            {
                _logger.LogError("Order not found  " + JsonSerializer.Serialize(orderToDelete));
                response.Status = StatusCodes.Status400BadRequest.ToString();
                response.Code = StatusCodes.Status400BadRequest;
                response.Message.Add("Order not found");
                return response;
            }
            Product product = await _productRepository.GetByIdAsync(orderToDelete.ProductId);
            try
            {
                await _orderRepository.DeleteAsync(orderToDelete);
                product.Stock += orderToDelete.Quantity;
                await _productRepository.UpdateAsync(product);
                response.Code = StatusCodes.Status200OK;
                response.Status = StatusCodes.Status200OK.ToString();
                response.Data = new { OrderID = orderToDelete.OrderId, ProductName = product.ProductName, OrderQuantity = orderToDelete.Quantity };
                response.Message.Add("Order is Deleted succesfully");
                _logger.LogInformation("Order is Deleted succesfully ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Code = StatusCodes.Status422UnprocessableEntity;
                response.Status = StatusCodes.Status422UnprocessableEntity.ToString();
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<CommonResponse> GetOrderDetails()
        {
            CommonResponse response = new CommonResponse();
           
            try
            {
                _logger.LogInformation("Order Details ");
                response.Status = StatusCodes.Status200OK.ToString();
                response.Code = StatusCodes.Status200OK;
                response.Data = await _orderRepository.GetOrderDetaisAsync();
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Code = StatusCodes.Status422UnprocessableEntity;
                response.Status = StatusCodes.Status422UnprocessableEntity.ToString();
                response.Message.Add(ex.Message);
            }
            return response;
        }

        public async Task<CommonResponse> CreateProduct(Product product)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (product != null) {
                
                    await _productRepository.CreateAsync(product);
                    response.Data = product;
                    response.Code = StatusCodes.Status200OK;
                    response.Message.Add("Product Create Successfully");
                    return response;
                }
            }
            catch (Exception ex) {

                _logger.LogError(ex.Message);
                response.Code = StatusCodes.Status422UnprocessableEntity;
                response.Status = StatusCodes.Status422UnprocessableEntity.ToString();
                response.Message.Add(ex.Message);
            }
            return response;
        }
    }
}

