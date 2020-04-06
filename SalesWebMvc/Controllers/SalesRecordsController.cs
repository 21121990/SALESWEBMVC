using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Data;
using System.Diagnostics;
using SalesWebMvc.Services.Exception;

namespace SalesWebMvc.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SalesRecordService _salesRecordService;
        private readonly SellerService _sellerService;
        private readonly ProductService _productService;

        public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService, ProductService productService)
        {
            _salesRecordService = salesRecordService;
            _sellerService = sellerService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _salesRecordService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }
        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _salesRecordService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }

        public async Task<IActionResult> SalesControl()
        {

            var salesRecord = await _salesRecordService.FindAllStatusAsync();
            var sellers = await _sellerService.FindAllAsync();
            var viewModel = new SellerFormViewModel { SellerColection = sellers, SalesRecords = salesRecord };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesControl([Bind("Id,Name")] Seller seller)
        {

            SellerFormViewModel viewModel = new SellerFormViewModel();
            SalesRecord sales = new SalesRecord();
            sales.Date = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
            sales.SellerId = seller.Id;
            sales.Status = Models.Enums.SalesStatus.Pendente;
            await _salesRecordService.InsertAsync(sales);
            viewModel.SalesRecords = await _salesRecordService.FindAllStatusAsync();
            viewModel.SellerColection = await _sellerService.FindAllAsync();
            return View(viewModel);
            
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }
            var obj = await _salesRecordService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não existe" });
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _salesRecordService.RemoveAsync(id);
                return RedirectToAction(nameof(SalesControl));
            }
            catch (IntegrityException e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });
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
        public async Task<IActionResult> SalesProducts(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }
            var products = await _productService.FindAllAsync();
            var SalesRepository = await _productService.FindAllSalesIdAsync(Id.Value);
            if (products == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não existe" });
            }
            var viewModel = new SellerFormViewModel { ProductsColetion = products };
            return View(viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesProducts(Product product, int id)
        {

            SellerFormViewModel viewModel = new SellerFormViewModel();
            SalesRecord sales = new SalesRecord();
            
            await _salesRecordService.InsertAsync(sales);
            viewModel.SalesRecords = await _salesRecordService.FindAllStatusAsync();
            viewModel.SellerColection = await _sellerService.FindAllAsync();
            return View(viewModel);

        }
    }
}