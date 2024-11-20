using ERP.Entities;

namespace ERP.Repositories
{
    public interface IProductRepository:IRepositoryBase<Product>
    {
      
        
    }

    public class ProductRepository: RepositoryBase<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }

    }
}
