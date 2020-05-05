using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index  ()
        {         
            
            if (HttpContext.Session.GetString("SessionUser")==null)
            {
                return RedirectToAction("Login", "Login");
            }
            var list = await _userService.FindAllAsync();
            return View(list);
        }
        public IActionResult Create()
        {
           
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
         
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!ModelState.IsValid)
            {
                var users = await _userService.FindAllAsync();
                var viewModel = new SellerFormViewModel { UserColection = users };
                return View(viewModel);
            }
            await _userService.InsertAsync(user);
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
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });
            }
            var obj = await _userService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
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
                await _userService.RemoveAsync(id);
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
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }
            var obj = await _userService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
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
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }
            var obj = await _userService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
     
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!ModelState.IsValid)
            {
                var viewModel = new SellerFormViewModel { User = user };
                return View(viewModel);
            }
            if (id != user.id)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id mismatch" });
            }
            try
            {
                await _userService.UpdateAsync(user);
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