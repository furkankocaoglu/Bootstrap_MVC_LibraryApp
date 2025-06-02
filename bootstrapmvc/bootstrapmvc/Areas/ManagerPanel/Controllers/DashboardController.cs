using bootstrapmvc.Areas.ManagerPanel.Filters;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    [ManagerLoginRequiredFilter]
    public class DashboardController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index()
        {
            var toplam = db.Books.Count();
            var aktif = db.Books.Count(x => x.IsDeleted == false);
            var silinmis = db.Books.Count(x => x.IsDeleted == true);
            var yazarSayisi = db.Books.Select(x => x.Writer).Distinct().Count();

            ViewBag.ToplamKitap = toplam;
            ViewBag.AktifKitap = aktif;
            ViewBag.SilinmisKitap = silinmis;
            ViewBag.YazarSayisi = yazarSayisi;

            var model = db.Books.ToList();
            return View(model);
        }
        public ActionResult _Index()
        {
            var toplamOgrenci = db.Students.Count();
            var aktifOgrenci = db.Students.Count(x => x.IsDeleted == false);
            var silinmisOgrenci = db.Students.Count(x => x.IsDeleted == true);
            var bolumSayisi = db.Students.Select(x => x.Department).Distinct().Count();

            ViewBag.ToplamOgrenci = toplamOgrenci;
            ViewBag.AktifOgrenci = aktifOgrenci;
            ViewBag.SilinmisOgrenci = silinmisOgrenci;
            ViewBag.BolumSayisi = bolumSayisi;

            
            var ogrenciler = db.Students.ToList();

            return View(ogrenciler);

        }
    }
}