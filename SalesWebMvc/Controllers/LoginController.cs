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
    public class LoginController : Controller
    {
        private readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            var result = await _userService.FindOneAsync(user.email);

            if (result == null)
            {
                return View();
           
            }
            else
            {
                HttpContext.Session.SetString("SessionUser", user.email);
                return RedirectToAction("SalesControl", "SalesRecords");
            }
            
        }
    }
}