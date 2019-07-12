using System;
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
using System.Collections.Generic;

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

        public IActionResult Dashboard()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var userEventCollection = db.GetCollection<CustomerEvent>("customerEvents");
                var userNonEventCollection = db.GetCollection<CustomerNonEvent>("customerNonEvents");
                var userEventCount = userEventCollection.FindAll().Count();
                var userNonEventCount = userNonEventCollection.FindAll().Count();
                ViewBag.userEventCount = userEventCount;
                ViewBag.userNonEventCount = userNonEventCount;
            }
            return PartialView("_AddEventDialogTemplate",ViewBag);
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

        [Authorize]
        public IActionResult Accounts()
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Account>("accounts");
                var results = query.FindAll();
                ViewBag.datasource = results.ToList();
            }
            return View();

        }

        public class Role
        {
            public Int32 Id { get; set; }
            public string RoleName { get; set; }
        }

        public IActionResult AddAccountPartial([FromBody] CRUDModel<Events> value)
        {
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Account>("accounts");
                var maxId = query.Max(x => x.Id);
                ViewBag.maxId = maxId.AsInt32 + 1;
                List<Role> roles = new List<Role>();
                roles.Add(new Role { Id = 1, RoleName = "Administrator" });
                roles.Add(new Role { Id = 2, RoleName = "Register" });
                ViewBag.roleList = roles;
            }
            return PartialView("_AddAccountDialogTemplate");
        }
        public IActionResult EditAccountPartial([FromBody] CRUDModel<Events> value)
        {
            Account result;
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var query = db.GetCollection<Account>("accounts");
                result = query.FindById(value.Value.Id);
                List<Role> roles = new List<Role>();
                roles.Add(new Role { Id = 1, RoleName = "Administrator" });
                roles.Add(new Role { Id = 2, RoleName = "Register" });
                ViewBag.roleList = roles;
                ViewBag.datasource = result;
            }
            return PartialView("_EditAccountDialogTemplate", result);
        }

        [HttpPost]
        public JsonResult AccountCreate(string Username, string Password, string RoleName)
        {

            try
            {
                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    var accounts = db.GetCollection<Account>("accounts");
                    var findSameName = accounts.Find(x => x.Username.Equals(Username));
                    if (findSameName.Count() > 0)
                    {
                        throw new InvalidOperationException("Username exist!");
                    }
                    else
                    {
                        CreateUser(Username, Password, RoleName);
                    }
                }                
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }

        public static bool CreateUser(string Username, string Password, string RoleName)
        {
            byte[] generatedSalt = PasswordHasher.GenerateSalt();
            byte[] hashedPassword = PasswordHasher.ComputeHash(Password, generatedSalt);
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                var accounts = db.GetCollection<Account>("accounts");
                var accountData = new Account
                {
                    Username = Username,
                    Password = hashedPassword,
                    Salt = generatedSalt,
                    RoleName = RoleName
                };
                accounts.Insert(accountData);
            }
            return true;
        }

        [HttpPost]
        public JsonResult AccountUpdate(Int32 Id, string Username, string Password, string RoleName)
        {

            try
            {
                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                   
                    

                    var accounts = db.GetCollection<Account>("accounts");
                    var accountToUpdate = accounts.FindById(Id);

                    if(accountToUpdate.Username.ToString() != Username)
                    {
                        var findSameName = accounts.Find(x => x.Username.Equals(Username));
                        if (findSameName.Count() > 0)
                        {
                            throw new InvalidOperationException("Username exist!");
                        }
                    }
                    else
                    {
                        UpdateUser(Id, Username, Password, RoleName);
                    }
                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
        }
        

        public static bool UpdateUser(Int32 Id, string Username, string Password, string RoleName)
        {
            
            using (var db = new LiteDatabase(@"Mineski.db"))
            {
                // Get customer collection
                var collection = db.GetCollection<Account>("accounts");
                var accountUpdate = collection.FindById(Id);
                              
                if (Password != null)
                {
                    byte[] generatedSalt = PasswordHasher.GenerateSalt();
                    byte[] hashedPassword = PasswordHasher.ComputeHash(Password, generatedSalt);
                    accountUpdate.Password = hashedPassword;
                    accountUpdate.Salt = generatedSalt;
                }

                accountUpdate.Username = Username;
                accountUpdate.RoleName = RoleName;
                collection.Update(accountUpdate);
            }
            return true;
        }

        [HttpPost]
        public JsonResult AccountDelete(Int32 Id)
        {

            try
            {
                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var collection = db.GetCollection<Account>("accounts");
                    collection.Delete(Id);

                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, responseText = exception.Message });
            }

            return Json(new { success = true });
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
            string curUrl = HttpContext.Request.QueryString.Value;
            string url = System.Net.WebUtility.UrlDecode(curUrl);
            var index = url.IndexOf('?', url.IndexOf('?') + 1);

            if(index > -1)
            {
                var ouput = string.Concat(url.Substring(0, index), "&", url.Substring(index + 1));
                var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(ouput);
                if (parsed["error"] != "")
                {
                    var error = parsed["error"].ToString();
                    ViewBag.error = error;
                }
            }            
                     
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

        [HttpPost]
        public IActionResult EventDelete(Int32 id)
        {
            try
            {

                using (var db = new LiteDatabase(@"Mineski.db"))
                {
                    // Get customer collection
                    var collection = db.GetCollection<Events>("events");
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