using ERP.Entities;
using ERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public interface IOrderRepository:IRepositoryBase<Order>
    {
        public Task<List<OrderDetais>> GetOrderDetaisAsync();
        public Task<List<TopCustomers>> GetTopCustomers();
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

        public async Task<List<TopCustomers>> GetTopCustomers()
        {
            return await _context.Orders.Select(x => new TopCustomers
            {
                CustomerName = x.CustomerName,
                Quantity = x.Quantity
            }).GroupBy(x => x.CustomerName)
            .Select(group => new TopCustomers { CustomerName = group.Key, Quantity = group.Sum(x => x.Quantity) })
            .OrderByDescending(x=>x.Quantity).Take(3)
            .ToListAsync();
        }
    }
}
