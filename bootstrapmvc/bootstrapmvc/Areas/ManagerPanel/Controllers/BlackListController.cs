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
    public class BlackListController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index()
        {
            var cezaKontrol = db.Borrows.Where(b => b.Penalty > 0).ToList();

            foreach (var borrow in cezaKontrol)
            {
                borrow.Student = db.Students.FirstOrDefault(s => s.ID == borrow.StudentID);
                borrow.Book = db.Books.FirstOrDefault(bk => bk.ID == borrow.BookID);
            }

            return View(cezaKontrol);
        }
        public ActionResult ForgivePenalty(int studentId)// admin ve mod rollerimiz var toplu affetme ve tekil affetme işlemleri sadece admin tarafından sağlanmaktadır.
        {
            var manager = Session["ManagerSession"] as Manager;

            if (manager == null || manager.ManagerRole_ID != 1)
            {
                TempData["mesaj"] = "Bu işlemi yapmaya yetkiniz yok. Admin girişi gereklidir.";
                return RedirectToAction("Index", "BlackList");
            }

            var ceza = db.Borrows.Where(b => b.StudentID == studentId && b.Penalty > 0).OrderByDescending(b => b.ReturnDate).FirstOrDefault();

            if (ceza == null)
            {
                TempData["mesaj"] = "Bu öğrencinin affedilecek cezası bulunmamaktadır.";
                return RedirectToAction("Index", "BlackList");
            }

            ceza.Penalty = 0;
            db.SaveChanges();

            TempData["mesaj"] = "Öğrencinin cezası affedildi.";
            return RedirectToAction("Index", "BlackList");
        }

        public ActionResult ForgiveAllPenalties()// admin ve mod rollerimiz var toplu affetme ve tekil affetme işlemleri sadece admin tarafından sağlanmaktadır.
        {
            var manager = Session["ManagerSession"] as Manager;

            if (manager == null || manager.ManagerRole_ID != 1)
            {
                TempData["mesaj"] = "Bu işlemi yapmaya yetkiniz yok. Admin girişi gereklidir.";
                return RedirectToAction("Index", "BlackList");
            }

            var cezalar = db.Borrows.Where(b => b.Penalty > 0).ToList();
            foreach (var c in cezalar)
            {
                c.Penalty = 0;
            }
            db.SaveChanges();

            TempData["mesaj"] = "Tüm öğrencilerin cezaları başarıyla affedildi.";
            return RedirectToAction("Index", "BlackList");
        }
    }

    
}