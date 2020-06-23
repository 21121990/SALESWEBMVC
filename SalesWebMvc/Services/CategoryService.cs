using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exception;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SalesWebMvc.Services
{
    public class CategoryService
    {

        private readonly SalesWebMvcContext _context;

        public CategoryService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> FindAllAsync()
        {
            var result = from obj in _context.Category select obj;

            return await result.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task InsertAsync(Category obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> FindByIdAsync(int id)
        {
            return await _context.Category.FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Category.FindAsync(id);
                _context.Category.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw new IntegrityException("Não é possível excluir uma Categoria que possui relacionamento");
            }

        }
        public async Task UpdateAsync(Category obj)
        {
            bool hasAny = await _context.Category.AnyAsync(x => x.Id == obj.Id);
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
