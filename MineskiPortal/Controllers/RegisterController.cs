using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using MineskiPortal.Models;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace MineskiPortal.Controllers
{
    public class RegisterController : Controller
    {
        public PartialViewResult Index()
        {
            return new PartialViewResult
            {
                ViewName = "Index",
                ViewData = this.ViewData
            };
        }

        public IActionResult Event()
        {
            return View();
        }

        public IActionResult NonEvent()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EventCreate(string Name, string Address, string DateOfBirth, string Email, string Gender, string MobileNumber, string MonthlyGamingExpense)
        {

            try
            {
                
                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var customerEvents = db.GetCollection<CustomerEvent>("customerEvents");

                    var customerEventData = new CustomerEvent
                    {
                        Address = Address,
                        DateOfBirth = DateOfBirth,
                        Email = Email,
                        Gender = Gender,
                        MobileNumber = MobileNumber,
                        MonthlyGamingExpense = MonthlyGamingExpense,
                        Name = Name,

                    };

                    // Insert new customer document (Id will be auto-incremented)
                    customerEvents.Insert(customerEventData);
                }
            }
            catch(Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot","uploads",
                        file.FileName);

            using (var stream = new FileStream(path, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }

        [HttpPost]
        public async Task<JsonResult> NonEventCreate(string Name, string Address, string Email, string Gender, string MobileNumber,string IdNumber, string MaritalStatus, IFormFile Photo)
        {

            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var customerEvents = db.GetCollection<CustomerNonEvent>("customerNonEvents");
                    if(Photo != null)
                    {
                        IActionResult actionResult = await UploadFile(Photo);
                    }

                    var customerNonEventData = new CustomerNonEvent
                    {
                        Address = Address,
                        Email = Email,
                        Gender = Gender,
                        MobileNumber = MobileNumber,
                        Name = Name,
                        IdNumber = IdNumber,
                        MaritalStatus = MaritalStatus,
                        Photo = Photo.FileName
                    };

                    // Insert new customer document (Id will be auto-incremented)
                    customerEvents.Insert(customerNonEventData);
                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }
    }
}