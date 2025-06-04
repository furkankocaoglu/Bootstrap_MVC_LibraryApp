using bootstrapmvc.Areas.ManagerPanel.Filters;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    [ManagerLoginRequiredFilter]
    public class DashboardController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index()//kitap istatistik
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
        public ActionResult _Index()//öğrenci istatistik
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
        public ActionResult BorrowStats()
        {
            var toplamOdunc = db.Borrows.Count();
            var teslimEdilen = db.Borrows.Count(x => x.IsReturned);
            var teslimEdilmeyen = db.Borrows.Count(x => !x.IsReturned);

            var today = DateTime.Now.Date;

            var gecikenler = db.Borrows.Count(b =>!b.IsReturned && DbFunctions.TruncateTime(b.DueDate) < today
            );

            ViewBag.ToplamOdunc = toplamOdunc;
            ViewBag.TeslimEdilen = teslimEdilen;
            ViewBag.TeslimEdilmeyen = teslimEdilmeyen;
            ViewBag.Geciken = gecikenler;

            var model = db.Borrows.Include("Student").Include("Book").ToList();

            return View(model);
        }
        public ActionResult BlackListStats()
        {
            var karaListe = db.Borrows.Where(b => b.Penalty > 0).Include("Student").ToList();

            ViewBag.KaraListeOgrenciSayisi = karaListe.Select(b => b.StudentID).Distinct().Count();

            ViewBag.ToplamCezaTutari = karaListe.Sum(b => b.Penalty);

            return View(karaListe);
        }
    }

    
}