using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWebMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace SalesWebMvc.Services
{
    public class ProductService
    {
        private readonly SalesWebMvcContext _context;

        public ProductService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> FindAllAsync()
        {
            return await _context.Product.OrderBy(x => x.Name).ToListAsync();
        }
        public async Task<List<SalesRepository>> FindAllSalesIdAsync(int id)
        {
            var result = from obj in _context.SalesRepository select obj;
           
            //inner join na consulta com include
            return await result
                .Include(x => x.ProductId)
                .Include(x => x.Product.Id)
                .OrderByDescending(x => x.Product.Name).Where(x => x.Id == id)               
                .ToListAsync();

           // return await _context.SalesRepository.Include(obj => obj.SalesRecordId == id).ToListAsync();
           // return await _context.SalesRecord.Include(obj => obj.Seller).FirstOrDefaultAsync(x => x.Id == id);

        }
    }
}
