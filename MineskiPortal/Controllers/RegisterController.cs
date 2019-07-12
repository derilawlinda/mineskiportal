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
using Syncfusion.EJ2.Base;
using Microsoft.AspNetCore.Authorization;

namespace MineskiPortal.Controllers
{
    public class RegisterController : Controller
    {
        [Authorize(Roles = "Administrator, Register")]
        public PartialViewResult Index()
        {
            return new PartialViewResult
            {
                ViewName = "Index",
                ViewData = this.ViewData
            };
        }

        [Authorize(Roles = "Administrator, Register")]
        public IActionResult Event(Int32 Id)
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                var result = query.FindById(Id);
                if(result == null)
                {
                    ViewBag.isEventRunning = false;
                }
                else
                {
                    ViewBag.eventName = result.EventName;
                    ViewBag.isEventRunning = true;
                }
            }

            return View();
        }

        [Authorize(Roles = "Administrator, Register")]
        public IActionResult ChooseEvent()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                var results = query.Find(q => q.DateTo.CompareTo(DateTime.Now) >= 0 && q.DateFrom.CompareTo(DateTime.Now) <= 0);
                ViewBag.events = results.ToList();
            }
            return View();
        }
        [Authorize(Roles = "Administrator, Register")]
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
                        Name = Name

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

            string photoFileName = "";
            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var customerEvents = db.GetCollection<CustomerNonEvent>("customerNonEvents");


                    if (Photo != null)
                    {
                        IActionResult actionResult = await UploadFile(Photo);
                        photoFileName = Photo.FileName;
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
                        Photo = photoFileName
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