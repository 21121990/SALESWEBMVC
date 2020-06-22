using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.ApiControllers.Products
{
    [ApiController]
    [Route("API/Products")]
    public class ApiProducts : Controller
    {
        private readonly SalesWebMvcContext _context;

        public ApiProducts(SalesWebMvcContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<List<Product>> products()
        {
            return await _context.Product.ToListAsync();
        }

    }
}
