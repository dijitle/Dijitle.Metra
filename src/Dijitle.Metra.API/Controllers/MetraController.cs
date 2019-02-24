using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dijitle.Metra.API.Controllers
{
    public class MetraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}