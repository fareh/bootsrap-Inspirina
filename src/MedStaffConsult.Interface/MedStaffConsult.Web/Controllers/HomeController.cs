using System.Collections.Generic;
using MedStaffConsult.Models;
using MedStaffConsult.Storage.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace MedStaffConsult.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepoStorage<Doctor> _docStorage;

        public HomeController(IRepoStorage<Doctor> docStorage)
        {
            _docStorage = docStorage;
        }
        public IActionResult Index()
        {
            var t = _docStorage.Get(new List<int>() { 1 });
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
