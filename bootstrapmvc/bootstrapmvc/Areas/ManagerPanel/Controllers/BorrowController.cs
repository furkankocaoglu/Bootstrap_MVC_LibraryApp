using bootstrapmvc.Areas.ManagerPanel.Data;
using bootstrapmvc.Areas.ManagerPanel.Filters;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    [ManagerLoginRequiredFilter]
    public class BorrowController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index(string searchName)
        {
            var searchBorrows = db.Borrows.Where(b => !b.IsReturned);

            if (!string.IsNullOrEmpty(searchName))
            {
                searchBorrows = searchBorrows.Where(b =>b.Student.Name.Contains(searchName) || b.Student.Surname.Contains(searchName));
            }

            var borrows = searchBorrows.Select(b => new BorrowViewModel
            {
                ID = b.ID,
                StudentName = b.Student.Name,
                StudentSurname = b.Student.Surname,
                StudentNumber = b.Student.StudentNumber,
                BookName = b.Book.Name,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                IsReturned = b.IsReturned

            }).ToList();

            ViewBag.SearchName = searchName;

            return View(borrows);
        }

        public ActionResult _Index()
        {
            var borrows = db.Borrows.Where(b => b.IsReturned).Select(b => new BorrowViewModel
            {
                ID = b.ID,
                StudentName = b.Student.Name,
                StudentSurname = b.Student.Surname,
                StudentNumber = b.Student.StudentNumber,
                BookName = b.Book.Name,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                IsReturned = b.IsReturned

            }).ToList();

            return View(borrows);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var oduncteOlanKitap = db.Borrows.Where(b => !b.IsReturned).Select(b => b.BookID).ToList();

            var aktifKitaplar = db.Books.Where(b => b.IsActive && !b.IsDeleted).ToList();
            var kitaplar = new List<Book>();

            foreach (var kitap in aktifKitaplar)
            {
                bool oduncteMi = db.Borrows.FirstOrDefault(b => !b.IsReturned && b.BookID == kitap.ID) != null;

                if (!oduncteMi)
                {
                    kitaplar.Add(kitap);
                }
            }

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
                        return RedirectToAction("Create", "Borrow");
                    }

                    DateTime today = DateTime.Today;

                    bool gecikmisOduncVar = false;
                    var borrows = db.Borrows.Where(b => b.StudentID == model.StudentID && !b.IsReturned).ToList();

                    foreach (var b in borrows)
                    {
                        if (b.DueDate.Date < today)
                        {
                            gecikmisOduncVar = true;
                            break;
                        }
                    }

                    if (gecikmisOduncVar)
                    {
                        TempData["mesaj"] = "Teslim süresi geçmiş kitabınız bulunmaktadır. Önce kitabı iade etmeniz ve 2 gün süreyi geçtiyse ceza ödemeniz gerekmektedir.";
                        return RedirectToAction("Create", "Borrow");
                    }

                    model.BorrowDate = DateTime.Now;
                    model.IsReturned = false;
                    model.Penalty = 0;

                    db.Borrows.Add(model);
                    db.SaveChanges();

                    TempData["mesaj"] = "Kitap ödünç verildi.";
                    return RedirectToAction("Index", "Borrow");
                }
                catch (Exception ex)
                {
                    TempData["mesaj"] = "Bir hata oluştu: " + ex.Message;
                }
            }

            var oduncteOlanKitap = db.Borrows.Where(b => !b.IsReturned).Select(b => b.BookID).ToList();

            var aktifKitaplarPost = db.Books.Where(b => b.IsActive && !b.IsDeleted).ToList();

            var kitaplarPost = new List<Book>();

            foreach (var kitap in aktifKitaplarPost)
            {
                if (!oduncteOlanKitap.Contains(kitap.ID))
                {
                    kitaplarPost.Add(kitap);
                }
            }

            ViewBag.StudentID = new SelectList(db.Students.Where(s => s.IsActive), "ID", "StudentNumber", model.StudentID);
            ViewBag.BookID = new SelectList(kitaplarPost, "ID", "Name", model.BookID);

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

            var gecmis = db.Borrows.Include(b => b.Student).Include(b => b.Book).Where(b => !b.IsReturned).ToList().Where(b => b.DueDate.Date < today).ToList();

            return View(gecmis);
        }
        public ActionResult NotOverdue()
        {
            DateTime today = DateTime.Today;

            var gecmemis = db.Borrows.Include(b => b.Student).Include(b => b.Book).Where(b => !b.IsReturned).ToList().Where(b => b.DueDate.Date >= today).ToList();

            return View(gecmemis);
        }
        public ActionResult StudentHistory(int? id)
        {
            var student = db.Students.FirstOrDefault(s => s.ID == id);

            var borrowDetails = db.Borrows.Where(b => b.StudentID == id).Select(b => new BorrowDetailViewModel
            {
                ID = b.ID,
                BookName = b.Book.Name,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                Penalty = b.Penalty,
                IsReturned = b.IsReturned

            }).ToList();

            var viewModel = new StudentBorrowHistoryViewModel
            {
                StudentID = student.ID,
                StudentName = student.Name,
                StudentSurname = student.Surname,
                BorrowHistory = borrowDetails,
                TotalPenalty = borrowDetails.Where(b => b.IsReturned && b.Penalty > 0).Sum(b => b.Penalty)
            };

            return View(viewModel);
        }
        public ActionResult StudentPenalties(int? id)
        {
            var student = db.Students.FirstOrDefault(s => s.ID == id);

            var borrowDetails = db.Borrows.Where(b => b.StudentID == id).Select(b => new BorrowDetailViewModel
            {
                BookName = b.Book.Name, 
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                Penalty = b.Penalty,
                IsReturned = b.IsReturned

            }).ToList();

            var totalPenalty = borrowDetails.Where(b => b.IsReturned && b.Penalty > 0).Sum(b => b.Penalty);

            var viewModel = new StudentBorrowHistoryViewModel
            {
                StudentID = student.ID,
                StudentName = student.Name,
                StudentSurname = student.Surname,
                BorrowHistory = borrowDetails,
                TotalPenalty = totalPenalty
            };

            return View(viewModel);
        }



    }

}