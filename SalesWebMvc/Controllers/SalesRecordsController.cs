using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exception;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
        public IActionResult Filter(DateTime? minDate, DateTime? maxDate)
        {
            if (minDate.HasValue && maxDate.HasValue)
            {
                return RedirectToAction("SalesControl", new { minDate, maxDate });
            }
            else
            {
                return RedirectToAction("SalesControl");
            }

        }

        public async Task<IActionResult> SalesControl(DateTime? minDate, DateTime? maxDate)
        {
            SellerFormViewModel viewModel = new SellerFormViewModel();
            if (!minDate.HasValue || !maxDate.HasValue)
            {
                viewModel.SalesRecords = await _salesRecordService.FindAllStatusAsync();
            }
            if (minDate.HasValue || maxDate.HasValue)
            {
                viewModel.SalesRecords = await _salesRecordService.FindByDateAsync(minDate, maxDate);
                ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
                ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            }

            viewModel.SellerColection = await _sellerService.FindAllAsync();

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesControl([Bind("Id,Name")] Seller seller)
        {
            try
            {
                SellerFormViewModel viewModel = new SellerFormViewModel();
                SalesRecord sales = new SalesRecord();
                sales.Date = Convert.ToDateTime(DateTime.Now);
                sales.SellerId = seller.Id;
                sales.Status = Models.Enums.SalesStatus.Pendente;
                await _salesRecordService.InsertAsync(sales);
                viewModel.SalesRecords = await _salesRecordService.FindAllStatusAsync();
                viewModel.SellerColection = await _sellerService.FindAllAsync();
                return View(viewModel);
            }
            catch (Exception e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
           

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
                await _salesRecordService.RemoveSalesRepositoryIdSalesAsync(id);
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
            var salesRecord = await _salesRecordService.FindSellerSalesAsync(Id.Value);
            if (salesRecord == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não existe" });
            }
            var salesRepository = await _productService.FindAllSalesIdAsync(Id.Value);
            var products = await _productService.FindAllAsync();
            Double totalSales = await _productService.FindTotalSalesAsync(Id.Value);
            ViewData["TotalSales"] = totalSales.ToString(format: "f2");
            var viewModel = new SellerFormViewModel { ProductsColetion = products, SalesRepositoryList = salesRepository, SalesRecord = salesRecord };
            return View(viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesProducts(Product product, int Id)
        {
            try
            {
                SellerFormViewModel viewModel = new SellerFormViewModel();
                SalesRepository salesRepository = new SalesRepository();
                SalesRecord salesRecord = new SalesRecord();
                salesRepository.SalesRecordId = Id;
                salesRepository.ProductId = product.Id;
                salesRecord = await _salesRecordService.FindByIdAsync(Id);
                await _salesRecordService.InsertSalesRepositoryAsync(salesRepository);

                //Calcular o total de produtos dentro da venda depois do insert para fazer o update.
                Double totalSales = await _productService.FindTotalSalesAsync(Id);
                ViewData["TotalSales"] = totalSales.ToString(format: "f2");
                await _salesRecordService.UpdateTotalSalesAsync(totalSales, salesRecord);
                viewModel.ProductsColetion = await _productService.FindAllAsync();
                viewModel.SalesRepositoryList = await _productService.FindAllSalesIdAsync(Id);
                viewModel.SalesRecord = await _salesRecordService.FindSellerSalesAsync(Id);
                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> DeleteSalesRepository(int Id, int id2)
        {
            try
            {
                SalesRecord salesRecord = new SalesRecord();
                salesRecord = await _salesRecordService.FindByIdAsync(id2);
                await _salesRecordService.RemoveSalesRepositoryIdProductAsync(Id);
                Double totalSales = await _productService.FindTotalSalesAsync(id2);
                await _salesRecordService.UpdateTotalSalesAsync(totalSales, salesRecord);

                return RedirectToAction("SalesProducts", "SalesRecords", new { @id = id2 });
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> UpdateStatus(SalesRecord salesRecord, int salesStatus)
        {
            try
            {
                var status = SalesStatus.Pendente;
                if (salesStatus == 0)
                {
                    status = SalesStatus.Pendente;
                }
                else if (salesStatus == 1)
                {
                    status = SalesStatus.Faturado;
                }
                else
                {
                    status = SalesStatus.Cancelado;
                }
                var result = await _salesRecordService.FindByIdAsync(salesRecord.Id);
                result.Status = status;
                await _salesRecordService.UpdateStatusAsync(result);
                return RedirectToAction("SalesControl");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}