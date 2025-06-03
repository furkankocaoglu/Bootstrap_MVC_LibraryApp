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
    public class BorrowController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index()
        {
            var borrows = db.Borrows.Where(b => !b.IsReturned).ToList();

            foreach (var borrow in borrows)
            {
                borrow.Student = db.Students.FirstOrDefault(s => s.ID == borrow.StudentID);
                borrow.Book = db.Books.FirstOrDefault(bk => bk.ID == borrow.BookID);
            }

            return View(borrows);
        }

        public ActionResult _Index()
        {
            var borrows = db.Borrows
                    .Where(b => b.IsReturned)
                    .ToList();

            foreach (var borrow in borrows)
            {
                borrow.Student = db.Students.FirstOrDefault(s => s.ID == borrow.StudentID);
                borrow.Book = db.Books.FirstOrDefault(bk => bk.ID == borrow.BookID);
            }

            return View(borrows);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var oduncteOlanKitapIDs = db.Borrows.Where(b => !b.IsReturned).Select(b => b.BookID).ToList();

            var kitaplar = db.Books.Where(b => b.IsActive && !b.IsDeleted && !oduncteOlanKitapIDs.Contains(b.ID)).ToList();

            ViewBag.StudentID = new SelectList(db.Students.Where(s => s.IsActive), "ID", "StudentNumber");
            ViewBag.BookID = new SelectList(kitaplar, "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Borrow model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.BorrowDate = DateTime.Now;
                    db.Borrows.Add(model);
                    db.SaveChanges();
                    TempData["mesaj"] = "Kitap ödünç verildi";
                    return RedirectToAction("Index", "Borrow");
                }
                catch
                {
                    ViewBag.mesaj = "Bir hata oluştu";
                }
            }

            var oduncteOlanKitapIDs = db.Borrows.Where(b => !b.IsReturned).Select(b => b.BookID).ToList();

            ViewBag.StudentID = new SelectList(db.Students.Where(s => s.IsActive), "ID", "StudentNumber", model.StudentID);
            ViewBag.BookID = new SelectList(db.Books.Where(b => b.IsActive && !b.IsDeleted && !oduncteOlanKitapIDs.Contains(b.ID)), "ID", "Name", model.BookID);

            return View(model);
        }
        public ActionResult Return(int? id)
        {
            Borrow borrow = db.Borrows.Find(id);

            borrow.IsReturned = true;
            borrow.ReturnDate = DateTime.Now;

            db.SaveChanges();
            TempData["mesaj"] = "Kitap iade alındı";
            return RedirectToAction("Index", "Borrow");
        }
    }

}