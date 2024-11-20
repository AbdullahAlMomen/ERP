using System.ComponentModel.DataAnnotations;

namespace ERP.Entities
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Stock { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
