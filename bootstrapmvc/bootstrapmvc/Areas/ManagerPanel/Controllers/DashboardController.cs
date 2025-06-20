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

            ViewBag.ToplamKitap = toplam;
            ViewBag.AktifKitap = aktif;
            ViewBag.SilinmisKitap = silinmis;

            var model = db.Books.ToList();
            return View(model);
        }
        public ActionResult _Index()//öğrenci istatistik
        {
            var toplamOgrenci = db.Students.Count();
            var aktifOgrenci = db.Students.Count(x => x.IsDeleted == false);
            var silinmisOgrenci = db.Students.Count(x => x.IsDeleted == true);

            ViewBag.ToplamOgrenci = toplamOgrenci;
            ViewBag.AktifOgrenci = aktifOgrenci;
            ViewBag.SilinmisOgrenci = silinmisOgrenci;

            var ogrenciler = db.Students.ToList();
            return View(ogrenciler);

        }
        public ActionResult BorrowStats()
        {
            var today = DateTime.Today;

            var toplamOdunc = db.Borrows.Count();
            var teslimEdilen = db.Borrows.Count(x => x.IsReturned);
            var teslimEdilmeyen = db.Borrows.Count(x => !x.IsReturned);
            var gecikenler = db.Borrows.Count(b => !b.IsReturned && b.DueDate < today);

            ViewBag.ToplamOdunc = toplamOdunc;
            ViewBag.TeslimEdilen = teslimEdilen;
            ViewBag.TeslimEdilmeyen = teslimEdilmeyen;
            ViewBag.Geciken = gecikenler;

            var model = db.Borrows.Include("Student").Include("Book").ToList();

            return View(model);
        }
        public ActionResult BlackListStats(string searchName)
        {
            var allKaraListe = db.Borrows.Where(b => b.Penalty > 0).ToList();

            var karaListeQuery = db.Borrows.Where(b => b.Penalty > 0);

            if (!string.IsNullOrEmpty(searchName))
            {
                karaListeQuery = karaListeQuery.Where(b => b.Student.Name.Contains(searchName) || b.Student.Surname.Contains(searchName));
            }

            var karaListe = karaListeQuery.Include(b => b.Student).Include(b => b.Book).ToList();

            ViewBag.KaraListeOgrenciSayisi = allKaraListe.Select(b => b.StudentID).Distinct().Count();
            ViewBag.ToplamCezaTutari = allKaraListe.Sum(b => b.Penalty);

            if (!string.IsNullOrEmpty(searchName))
            {
                ViewBag.FiltreliCezaTutari = karaListeQuery.Sum(b => (decimal?)b.Penalty) ?? 0;
            }
            else
            {
                ViewBag.FiltreliCezaTutari = 0;
            }

            ViewBag.SearchName = searchName;

            return View(karaListe);
        }

    }

    
}