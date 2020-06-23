using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    
    public class ProductsController : Controller
    {
       
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductsController(ProductService productService, CategoryService categoryService)
        {
          
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: product

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var list = await _productService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var category = await _categoryService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Categories = category };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!ModelState.IsValid)
            {
                var category = await _categoryService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Product = product, Categories = category };
                return View(viewModel);
            }
            await _productService.InsertAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }
            var obj = await _productService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            try
            {
                await _productService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });
            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não fornecido" });
            }
            var obj = await _productService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não encontrado" });
            }
            return View(obj);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não fornecido" });
            }
            var obj = await _productService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não encotrado" });
            }
            List<Category> categories = await _categoryService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Product = obj, Categories = categories };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Product = product, Categories = categories };
                return View(viewModel);
            }
            if (id != product.Id)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não encotrado" });
            }
            try
            {
                await _productService.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {

                return RedirectToAction(nameof(Error), new { Message = e.Message });
            }

        }
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier

            };
            return View(viewModel);
        }


    }

}
