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
            var borrows = db.Borrows.Where(b => b.IsReturned).ToList();

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
            var oduncteOlanKitap = db.Borrows.Where(b => !b.IsReturned).Select(b => b.BookID).ToList();

            var kitaplar = db.Books.Where(b => b.IsActive && !b.IsDeleted && !oduncteOlanKitap.Contains(b.ID)).ToList();

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
                    decimal toplamCeza = db.Borrows.Where(b => b.StudentID == model.StudentID).Sum(b => (decimal?)b.Penalty) ?? 0;

                    if (toplamCeza > 50)
                    {
                        TempData["mesaj"] = "Toplam cezanız 50 TL'yi geçtiği için kitap alamazsınız. Önce geçmiş cezayı ödeyiniz.";
                        return RedirectToAction("Create","Borrow");
                    }

                    DateTime today = DateTime.Today;

                    bool gecikmisOduncVar = db.Borrows.Any(
                        b => b.StudentID == model.StudentID &&
                             !b.IsReturned &&
                             DbFunctions.TruncateTime(b.DueDate) < today
                    );

                    if (gecikmisOduncVar)
                    {
                        TempData["mesaj"] = "Teslim süresi geçmiş kitabınız bulunmaktadır. Önce kitabı iade etmeniz gerekmektedir.";
                        return RedirectToAction("Create", "Borrow");
                    }

                    model.BorrowDate = DateTime.Now;
                    model.IsReturned = false;
                    model.Penalty = 0;

                    db.Borrows.Add(model);
                    db.SaveChanges();

                    TempData["mesaj"] = "Kitap ödünç verildi.";
                    return RedirectToAction("Index","Borrow");
                }
                catch (Exception ex)
                {
                    TempData["mesaj"] = "Bir hata oluştu: " + ex.Message;
                }
            }

            var oduncteOlanKitap = db.Borrows.Where(b => !b.IsReturned) .Select(b => b.BookID).ToList();

            ViewBag.StudentID = new SelectList(db.Students.Where(s => s.IsActive), "ID", "StudentNumber", model.StudentID);
            ViewBag.BookID = new SelectList(db.Books.Where(b => b.IsActive && !b.IsDeleted && !oduncteOlanKitap.Contains(b.ID)), "ID", "Name", model.BookID);

            return View(model);
        }
        public ActionResult Return(int? id)
        {
            if (id != null)
            {
                Borrow borrow = db.Borrows.Find(id);
                if (borrow != null)
                {
                    borrow.IsReturned = true;
                    borrow.ReturnDate = DateTime.Now;

                    // Teslim günü 23:59:59’a kadar toleranslı olacak şekilde kontrol ediyorum.
                    DateTime dueEndOfDay = borrow.DueDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                    if (borrow.ReturnDate > dueEndOfDay)
                    {
                        int daysLate = (borrow.ReturnDate.Value.Date - borrow.DueDate.Date).Days;
                        borrow.Penalty = daysLate * 25;
                        TempData["mesaj"] = $"Kitap iade alındı, {daysLate} gün gecikme nedeniyle ceza uygulandı.";
                    }
                    else
                    {
                        borrow.Penalty = 0;
                        TempData["mesaj"] = "Kitap iade alındı.";
                    }

                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Borrow");
        }
        public ActionResult Overdue()
        {
            DateTime today = DateTime.Today;

            var gecmis = db.Borrows.Include(b => b.Student).Include(b => b.Book).Where(b => !b.IsReturned && DbFunctions.TruncateTime(b.DueDate) < today).ToList();

            return View(gecmis);
        }
        public ActionResult NotOverdue()
        {
            DateTime today = DateTime.Today;

            var gecmemis = db.Borrows.Include(b => b.Student).Include(b => b.Book).Where(b => !b.IsReturned && DbFunctions.TruncateTime(b.DueDate) >= today).ToList(); //DbFunctions.TruncateTime(b.DueDate) Bu, DueDate'in saat kısmını yok sayar. Sadece tarihi alır:


            return View(gecmemis);
        }


    }

}