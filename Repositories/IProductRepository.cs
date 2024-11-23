using ERP.Entities;
using ERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public interface IProductRepository:IRepositoryBase<Product>
    {
        public Task<List<ProductRevenueSummary>> GetProductRevenueSummaries();
        public Task<List<ProductDetails>> GetProductDetails(decimal threshold);
        public Task<List<Product>> GetProductWithNoOrder();
    }

    public class ProductRepository: RepositoryBase<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
        public async Task<List<ProductRevenueSummary>> GetProductRevenueSummaries()
        {
            return await _context.Products
                            .Join(_context.Orders,
                                  product => product.ProductID, 
                                  order => order.ProductId,    
                                  (product, order) => new ProductRevenueSummary
                                  {
                                     ProductName= product.ProductName,
                                     TotalQuantity= order.Quantity,
                                      TotalRevenue = order.Quantity * product.UnitPrice
                                  })
                            .GroupBy(x => x.ProductName)    
                            .Select(group => new ProductRevenueSummary
                            {
                                ProductName = group.Key,
                                TotalQuantity = group.Sum(x => x.TotalQuantity),
                                TotalRevenue = group.Sum(x => x.TotalRevenue)
                            })
                            .ToListAsync();
        }

        public async Task<List<ProductDetails>>GetProductDetails(decimal threshold)
        {
            return await _context.Products.Select(x => new ProductDetails
            {
                ProductName=x.ProductName,
                ProdcutPrice=x.UnitPrice,
                Stocks=x.Stock
            }).Where(x => x.Stocks <= threshold).ToListAsync();
        }

        public async Task<List<Product>> GetProductWithNoOrder()
        {
            return await _context.Products.
                GroupJoin(_context.Orders,
                product => product.ProductID,
                order => order.ProductId,
                (product, order) => new { Product = product, Order = order }).Where(x => !x.Order.Any()).Select(x => x.Product).ToListAsync();
        }

    }
}
