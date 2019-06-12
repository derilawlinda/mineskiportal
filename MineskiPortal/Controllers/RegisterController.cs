using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
    }
}