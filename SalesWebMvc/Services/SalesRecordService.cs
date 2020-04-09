using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWebMvc.Models;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exception;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            //inner join na consulta com include
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            //inner join na consulta com include
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Seller.Department)
                .ToListAsync();
        }
        public async Task InsertAsync(SalesRecord salesRecord)
        {
            _context.Add(salesRecord);
            await _context.SaveChangesAsync();
        }
        public async Task InsertSalesRepositoryAsync(SalesRepository salesRepository)
        {
            //Adicionar o metodo que faz insert de produtos na venda e o valor na venda.
            
            _context.Add(salesRepository);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveSalesRepositoryIdProductAsync(int Id)
        {
            var salesRepository = await _context.SalesRepository.FindAsync(Id);
            _context.SalesRepository.Remove(salesRepository);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveSalesRepositoryIdSalesAsync(int Id)
        {
            var salesRepository = await _context.SalesRepository.Where(x=>x.SalesRecordId == Id).ToListAsync();

            foreach (var item in salesRepository)
            {
                _context.SalesRepository.Remove(item);
            }                    
            
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTotalSalesAync(double totalSales, SalesRecord salesRecord)
        {
            //Adicionar o metodo que faz insert de produtos na venda e o valor na venda.
            salesRecord.Amount = totalSales;
            _context.SalesRecord.Update(salesRecord);
            await _context.SaveChangesAsync();
        }
        public async Task<SalesRecord> FindSellerSalesAsync(int id)
        {
            return await _context.SalesRecord.Include(obj => obj.Seller).FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<List<SalesRecord>> FindAllStatusAsync()
        {          
            return  await _context.SalesRecord.Where(x => x.Status == Models.Enums.SalesStatus.Pendente).ToListAsync();
        }
        public async Task<SalesRecord> FindByIdAsync(int id)
        {
            return await _context.SalesRecord.Include(obj => obj.Seller).FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.SalesRecord.FindAsync(id);
                _context.SalesRecord.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {

                throw new IntegrityException(e.Message);
            }

        }

    }
}
