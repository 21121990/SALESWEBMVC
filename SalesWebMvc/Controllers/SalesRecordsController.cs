using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Data;

namespace SalesWebMvc.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SalesRecordService _salesRecordService;
        private readonly SellerService _sellerService;

        public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService)
        {
            _salesRecordService = salesRecordService;
            _sellerService = sellerService;
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
            var sellers = await _sellerService.FindAllAsync();
            var salesRecord = await _salesRecordService.FindAllAsyncDate(DateTime.Now);
            var viewModel = new SellerFormViewModel { SellerColection = sellers, SalesRecords = salesRecord };
                        
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesControl([Bind("Id,Name")] Seller seller)
        {
            try
            {
                if (seller != null)
                {
                   
                    SalesRecord sales = new SalesRecord();
                    sales.Date = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"));
                    sales.SellerId = seller.Id;
                    sales.Status = Models.Enums.SalesStatus.Pendente;
                    await _salesRecordService.InsertAsync(sales);
                    SellerFormViewModel viewModel = new SellerFormViewModel();
                    viewModel.SalesRecords = await _salesRecordService.FindAllAsyncDate(DateTime.Now);
                    viewModel.SellerColection = await _sellerService.FindAllAsync();
                    return View(viewModel);
                }
                else
                {
                    var sellers = await _sellerService.FindAllAsync();
                    var salesRecord = await _salesRecordService.FindAllAsyncDate(DateTime.Now);
                    var viewModel = new SellerFormViewModel { SellerColection = sellers, SalesRecords = salesRecord };
                    return View(viewModel);

                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }


        }
    }
}