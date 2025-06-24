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
        public ActionResult Index()
        {
            int toplam = db.Books.Count();
            int aktif = db.Books.Count(x => x.IsDeleted == false);
            int silinmis = db.Books.Count(x => x.IsDeleted == true);

            ViewBag.ToplamKitap = toplam;
            ViewBag.AktifKitap = aktif;
            ViewBag.SilinmisKitap = silinmis;

            List<Book> model = db.Books.ToList();

            return View(model);
        }
        public ActionResult _Index()
        {
            int toplamOgrenci = db.Students.Count();
            int aktifOgrenci = db.Students.Count(x => x.IsDeleted == false);
            int silinmisOgrenci = db.Students.Count(x => x.IsDeleted == true);

            ViewBag.ToplamOgrenci = toplamOgrenci;
            ViewBag.AktifOgrenci = aktifOgrenci;
            ViewBag.SilinmisOgrenci = silinmisOgrenci;

            List<Student> ogrenciler = db.Students.ToList();
            return View(ogrenciler);
        }
        public ActionResult BorrowStats()
        {
            DateTime today = DateTime.Today;

            int toplamOdunc = db.Borrows.Count();
            int teslimEdilen = db.Borrows.Count(x => x.IsReturned);
            int teslimEdilmeyen = db.Borrows.Count(x => !x.IsReturned);
            int gecikenler = db.Borrows.Count(b => !b.IsReturned && b.DueDate < today);

            ViewBag.ToplamOdunc = toplamOdunc;
            ViewBag.TeslimEdilen = teslimEdilen;
            ViewBag.TeslimEdilmeyen = teslimEdilmeyen;
            ViewBag.Geciken = gecikenler;

            List<Borrow> model = db.Borrows.Include("Student").Include("Book").ToList();

            return View(model);
        }
        public ActionResult BlackListStats(string searchName)
        {
            List<Borrow> allKaraListe = db.Borrows.Where(b => b.Penalty > 0).ToList();

            List<Borrow> karaListeQuery = db.Borrows.Where(b => b.Penalty > 0).Include(b => b.Student).Include(b => b.Book).ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                karaListeQuery = karaListeQuery.Where(b => b.Student.Name.Contains(searchName) || b.Student.Surname.Contains(searchName)).ToList();
            }

            List<Borrow> karaListe = karaListeQuery;

            ViewBag.KaraListeOgrenciSayisi = allKaraListe.Select(b => b.StudentID).Distinct().Count();
            ViewBag.ToplamCezaTutari = allKaraListe.Sum(b => b.Penalty);

            if (!string.IsNullOrEmpty(searchName))
            {
                ViewBag.FiltreliCezaTutari = karaListe.Sum(b => b.Penalty);
            }
            else
            {
                ViewBag.FiltreliCezaTutari = 0;
            }

            ViewBag.SearchName = searchName;

            return View(karaListe);

           /*var allKaraListe = db.Borrows.Where(b => b.Penalty > 0).ToList();

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

            return View(karaListe); */


        }


    }

    
}