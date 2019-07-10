﻿using System;
using System.Linq;
using System.Security.Claims;
using LiteDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MineskiPortal.Models;
using Syncfusion.EJ2.Base;
using MineskiPortal.Helpers;
using System.Globalization;

namespace MineskiPortal.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            if(User.Identity.Name != null)
            {
                ViewBag.userName = User.Identity.Name;
            }
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var userEventCollection = db.GetCollection<CustomerEvent>("customerEvents");
                var userNonEventCollection = db.GetCollection<CustomerNonEvent>("customerNonEvents");
                var userEventCount = userEventCollection.FindAll().Count();
                var userNonEventCount = userNonEventCollection.FindAll().Count();
                ViewBag.userEventCount = userEventCount;
                ViewBag.userNonEventCount = userNonEventCount;
            }
            return View();

        }

        [Authorize]
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

        [Authorize]
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

        public static bool CreateUser(string username, string password)
        {
            byte[] generatedSalt = PasswordHasher.GenerateSalt();
            byte[] hashedPassword = PasswordHasher.ComputeHash(password,generatedSalt);
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var accounts = db.GetCollection<Account>("accounts");

                var accountData = new Account
                {
                    Username = username,
                    Password = hashedPassword,
                    Salt = generatedSalt
                };
                accounts.Insert(accountData);
            }
            return true;
        }

        [Authorize]
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
            string error = HttpContext.Request.Query["error"];
            ViewBag.error = error;
            return View();

        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            //Check the user name and password  
            //Here can be implemented checking logic from the database  
            var db = new LiteDatabase(@"Mineski.db");
            var query = db.GetCollection<Account>("accounts");
            Account result = query.Find(Query.EQ("Username", userName)).FirstOrDefault();

            if(result == null)
            {
                return RedirectToAction("", "Dashboard", new
                {
                    error = "InvalidUsername"
                });
            }

            if (PasswordHasher.VerifyPassword(password, result.Salt, result.Password))
            {

                //Create the identity for the user  
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
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
                        DateFrom = DateTime.ParseExact(DateFrom, "d/M/yyyy", CultureInfo.InvariantCulture),
                        DateTo = DateTime.ParseExact(DateTo, "d/M/yyyy", CultureInfo.InvariantCulture),
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
                    eventUpdate.DateFrom = DateTime.ParseExact(DateFrom, "d/M/yyyy", CultureInfo.InvariantCulture);
                    eventUpdate.DateTo = DateTime.ParseExact(DateTo, "d/M/yyyy", CultureInfo.InvariantCulture);
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

        [HttpPost]
        public IActionResult CustomerEventDelete(Int32 id)
        {
            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var collection = db.GetCollection<CustomerEvent>("customerEvents");
                    collection.Delete(id);
                    
                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CustomerNonEventDelete(Int32 id)
        {
            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var collection = db.GetCollection<CustomerNonEvent>("customerNonEvents");
                    collection.Delete(id);

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