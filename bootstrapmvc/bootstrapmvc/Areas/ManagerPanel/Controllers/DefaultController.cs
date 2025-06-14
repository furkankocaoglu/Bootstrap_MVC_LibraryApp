using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    public class DefaultController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index()
        {
            return View();
        }
    }
}