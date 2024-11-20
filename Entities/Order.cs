namespace ERP.Entities
{
    public class Order
    {
        public int OrderId {  get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string CustomerName { get;set; }
        public decimal Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
