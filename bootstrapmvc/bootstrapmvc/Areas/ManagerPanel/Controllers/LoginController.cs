using bootstrapmvc.Areas.ManagerPanel.Data;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    public class LoginController : Controller
    {
        Model1 db= new Model1();
        public ActionResult Index()
        {
            if (Request.Cookies["ManagerCookie"] != null)
            {
                HttpCookie SavedCookie = Request.Cookies["ManagerCookie"];
                string mail = SavedCookie.Values["mail"];
                string password = SavedCookie.Values["password"];

                Manager m = db.Managers.FirstOrDefault(x => x.Mail == mail && x.Password == password);
                if (m != null)
                {
                    if (m.IsActive)
                    {
                        Session["ManagerSession"] = m;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(new ManagerLoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ManagerLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Manager m = db.Managers.FirstOrDefault(x => x.Mail == model.Mail && x.Password == model.Password);
                if (m != null)
                {
                    if (m.IsActive)
                    {
                        if (model.RememberMe)
                        {
                            HttpCookie cookie = new HttpCookie("ManagerCookie");
                            cookie["mail"] = model.Mail;
                            cookie["password"] = model.Password;
                            cookie.Expires = DateTime.Now.AddDays(10);
                            Response.Cookies.Add(cookie);
                        }
                        Session["ManagerSession"] = m;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.mesaj = "Kullanıcı hesabınız askıya alınmıştır";
                    }
                }
                else
                {
                    ViewBag.mesaj = "Kullanıcı bulunamadı";
                }
            }
            return View(model);
        }
        public ActionResult LogOut()
        {
            Session["ManagerSession"] = null;
            if (Request.Cookies["ManagerCookie"] != null)
            {
                HttpCookie SavedCookie = Request.Cookies["ManagerCookie"];
                SavedCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(SavedCookie);
            }

            return RedirectToAction("Index", "Default");
        }
    }
}