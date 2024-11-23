using System.Net;

namespace ERP.Models
{
    public class OrderDetais
    {
        public string ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string CustomerName { get; set; }
    }

    public class ProductRevenueSummary
    {
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public class ProductDetails
    {
        public string ProductName { get; set; }
        public decimal ProdcutPrice { get; set; }   
        public decimal? Stocks { get; set; }
    }

    public class TopCustomers
    {
        public string CustomerName { get; set; }
        public decimal? Quantity { get; set; }
    }
}
