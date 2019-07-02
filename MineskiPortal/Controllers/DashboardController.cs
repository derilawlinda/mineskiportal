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
                var query = db.GetCollection<CustomerNonEvent>("customerNonEvents");
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
                var maxId = query.Max(x => x.Id);
                ViewBag.maxId = maxId.AsInt32 + 1;
            }
            return PartialView("_AddEventDialogTemplate");
        }



        public IActionResult EditEventPartial([FromBody] CRUDModel<Events> value)
        {
            Events result;
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Events>("events");
                result = query.FindById(value.Value.Id);
                ViewBag.datasource = result;
            }
            return PartialView("_EditEventDialogTemplate", result);
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
                        DateFrom = Convert.ToDateTime(DateFrom).AddHours(7),
                        DateTo = Convert.ToDateTime(DateTo).AddHours(7),
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
                    var collection = db.GetCollection<Events>("events");
                    var eventUpdate = collection.FindById(Id);
                    eventUpdate.EventName = EventName;
                    eventUpdate.DateFrom = Convert.ToDateTime(DateFrom).AddHours(7);
                    eventUpdate.DateTo = Convert.ToDateTime(DateTo).AddHours(7);
                    eventUpdate.Description = Description;
                    collection.Update(eventUpdate);
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