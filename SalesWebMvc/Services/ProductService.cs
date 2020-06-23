using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exception;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            // var salesRespository = await _context.SalesRepository.Include(x => x.SalesRecordId == id).ToListAsync();
            //var product = await _context.Product.OrderBy(x => x.Name).ToListAsync();

            var result = from obj in _context.SalesRepository select obj;

            //inner join na consulta com include
            return await result.Where(x => x.SalesRecordId == id)
                 .ToListAsync();

        }
        public async Task<double> FindTotalSalesAsync(int id)
        {


            var result = from obj in _context.SalesRepository select obj;

            //inner join na consulta somando valores
            return await result.Where(x => x.SalesRecordId == id).SumAsync(x => x.Product.Value);

        }
        public async Task InsertAsync(Product obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }
        public async Task<Product> FindByIdAsync(int id)
        {
            return await _context.Product.Include(obj => obj.Category).FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Product.FindAsync(id);
                _context.Product.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw new IntegrityException("Não é possível excluir um produto que possui vendas");
            }

        }
        public async Task UpdateAsync(Product obj)
        {
            bool hasAny = await _context.Product.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbConcurrencyException e)
            {

                throw new DbConcurrencyException(e.Message);
            }

        }
    }
}
