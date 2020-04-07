﻿using System;
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

            //inner join na consulta com include
            return await result.Where(x => x.SalesRecordId == id).SumAsync(x => x.Product.Value);

        }
    }
}
