using ERP.Entities;
using ERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public interface IOrderRepository:IRepositoryBase<Order>
    {
        public Task<List<OrderDetais>> GetOrderDetaisAsync();
    }
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderDetais>> GetOrderDetaisAsync()
        {
            return await _context.Orders.Include(x => x.Product).Select(x => new OrderDetais
            {
                ProductName=x.Product.ProductName,
                ProductPrice=x.Product.UnitPrice,
                OrderDate=x.OrderDate,
                OrderQuantity=x.Quantity,
                CustomerName=x.CustomerName
            }).OrderByDescending(x=>x.OrderDate).ToListAsync();
        }
    }
}
