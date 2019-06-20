using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using MineskiPortal.Models;

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

            }

            return Json(String.Format("'Success': 'true'"));
        }
    }
}