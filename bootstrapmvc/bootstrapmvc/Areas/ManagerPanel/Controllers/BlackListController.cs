using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
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
        public ActionResult ForgivePenalty(int studentId)
        {
            var cezalar = db.Borrows.Where(b => b.StudentID == studentId && b.Penalty > 0).ToList();

            if (cezalar.Count == 0)
            {
                TempData["mesaj"] = "Bu öğrencinin affedilecek cezası bulunmamaktadır.";
                return RedirectToAction("Index","BlackList");
            }

            foreach (var borrow in cezalar)
            {
                borrow.Penalty = 0;
            }

            db.SaveChanges();
            TempData["mesaj"] = "Öğrencinin cezası başarıyla affedildi.Yeni ödünç girişi yapabilirsiniz.";
            return RedirectToAction("Index", "BlackList");
        }



    }
}