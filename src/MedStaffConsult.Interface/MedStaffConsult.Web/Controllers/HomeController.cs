using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            await _docStorage.Set(new Doctor
            {
                UId = 5,
                Name = "BEN AISSA",
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now
            });
            var t = await _docStorage.Find(doctor => doctor.Name == "BEN AISSA");
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
