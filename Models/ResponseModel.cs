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
}
