using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using MineskiPortal.Models;
using Syncfusion.EJ2.Base;

namespace MineskiPortal.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();

        }

        public IActionResult UserNonEvent()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<CustomerEvent>("customerNonEvents");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return View();

        }

        public IActionResult UserEvent()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<CustomerEvent>("customerEvents");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return View();

        }

        public IActionResult Events()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return View();

        }

        public IActionResult AddEventPartial([FromBody] CRUDModel<Events> value)
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return PartialView("_AddEventDialogTemplate");
        }



        public IActionResult EditEventPartial([FromBody] CRUDModel<Events> value)
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return PartialView("_EditEventDialogTemplate",value.Value);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EventCreate(string EventName, string DateFrom, string DateTo, string Description)
        {

            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var events = db.GetCollection<Events>("events");

                    var eventData = new Events
                    {
                        EventName = EventName,
                        DateFrom = DateFrom,
                        DateTo = DateTo,
                        Description = Description

                    };

                    // Insert new customer document (Id will be auto-incremented)
                    events.Insert(eventData);
                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EventUpdate(Int32 Id, string EventName, string DateFrom, string DateTo, string Description)
        {

            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var events = db.GetCollection<Events>("events");

                    var eventData = new Events
                    {
                        Id = Id,
                        EventName = EventName,
                        DateFrom = DateFrom,
                        DateTo = DateTo,
                        Description = Description

                    };

                    // Insert new customer document (Id will be auto-incremented)
                    events.Update(eventData);
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