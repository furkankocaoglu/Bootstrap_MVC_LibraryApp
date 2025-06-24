using bootstrapmvc.Areas.ManagerPanel.Filters;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    [ManagerLoginRequiredFilter]
    public class BookController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index(string searchName)
        {
            List<Book> bookSearch = db.Books.Where(x => x.IsDeleted == false).ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                bookSearch = bookSearch.Where(x => x.Name.Contains(searchName)).ToList();
            }

            ViewBag.SearchName = searchName;

            return View(bookSearch);

        }
        public ActionResult _Index(string searchName)
        {
            List<Book> bookSearch = db.Books.Where(x => x.IsDeleted == true).ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                bookSearch = bookSearch.Where(x => x.Name.Contains(searchName)).ToList();
            }

            ViewBag.SearchName = searchName;

            return View(bookSearch);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Book model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Books.Add(model);
                    db.SaveChanges();
                    TempData["mesaj"] = "Kitap başarılı bir şekilde eklendi.";
                    return RedirectToAction("Index", "Book");

                }
                catch
                {
                    TempData["mesaj"] = "Bir hata oluştu";
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                Book c = db.Books.Find(id);
                if (c != null)
                {
                    return View(c);
                }
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpPost]
        public ActionResult Edit(Book model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["mesaj"] = "Kitap güncelleme başarılı";
                    return RedirectToAction("Index", "Book");
                }
                catch
                {
                    TempData["mesaj"] = "Bir hata oluştu";
                }
            }
            return View(model);
        }
        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                Book c = db.Books.Find(id);
                if (c != null)
                {
                    c.IsDeleted = true;
                    c.IsActive = false;
                    db.SaveChanges();
                    TempData["mesaj"] = "Kitap silme işlemi başarılı";
                }
            }
            return RedirectToAction("Index", "Book");
        }
        public ActionResult Back(int? id)
        {
            if (id != null)
            {
                Book c = db.Books.Find(id);
                if (c != null)
                {
                    c.IsDeleted = false;
                    c.IsActive = true;
                    db.SaveChanges();
                    TempData["mesaj"] = "Kitap aktif etme işlemi başarılı";
                }
            }
            return RedirectToAction("Index", "Book");
        }
        public ActionResult ActivateAll()
        {
            List<Book> silinmisKitaplar = db.Books.Where(c => c.IsDeleted == true).ToList();

            foreach (Book kitap in silinmisKitaplar)
            {
                kitap.IsActive = true;
                kitap.IsDeleted = false;
            }
            db.SaveChanges();
            TempData["mesaj"] = "Tüm kitaplar başarıyla aktifleştirildi.";
            return RedirectToAction("Index", "Book");
        }
    }
}