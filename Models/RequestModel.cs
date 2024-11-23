using System.ComponentModel.DataAnnotations;

namespace ERP.Models
{
    public class OrderUpdateRequestModel
    {
        public int orderId {  get; set; }
        public decimal orderQuantity { get; set; }
    }
    public class CreateOrderRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
    }
    public class CreateProductRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } 

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal UnitPrice { get; set; } 

        [Range(0, double.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public decimal? Stock { get; set; } 
    }
}
