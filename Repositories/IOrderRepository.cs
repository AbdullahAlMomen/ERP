using ERP.Entities;

namespace ERP.Repositories
{
    public interface IOrderRepository:IRepositoryBase<Order>
    {
    }
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
