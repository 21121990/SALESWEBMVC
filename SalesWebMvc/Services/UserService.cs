using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exception;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SalesWebMvc.Services
{
    public class UserService
    {
        private readonly SalesWebMvcContext _context;

        public UserService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<User> FindOneAsync(string email, string password)
        {
 
            return await _context.User.FirstOrDefaultAsync(x => x.email == email && x.password == password);

        }
        public async Task<List<User>> FindAllAsync()
        {
            var result = from obj in _context.User select obj;

            return await result.OrderBy(x => x.id).ToListAsync();
        }

        public async Task InsertAsync(User obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);

        }
        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.User.FindAsync(id);
                _context.User.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw new IntegrityException("Não é possível excluir um vendedor que possui vendas");
            }

        }
        public async Task UpdateAsync(User obj)
        {
            bool hasAny = await _context.User.AnyAsync(x => x.id == obj.id);
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
