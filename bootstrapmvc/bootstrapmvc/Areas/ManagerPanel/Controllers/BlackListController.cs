using bootstrapmvc.Areas.ManagerPanel.Data;
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
        public ActionResult Index(string searchName)
        {
            List<BlackListPenaltyViewModel> blackList = db.Borrows.Where(b => b.Penalty > 0 && (string.IsNullOrEmpty(searchName) || b.Student.Name.Contains(searchName) || b.Student.Surname.Contains(searchName))).Select(b => new BlackListPenaltyViewModel
            {
                BorrowID = b.ID,
                Penalty = b.Penalty,
                StudentName = b.Student.Name,
                StudentSurname = b.Student.Surname,
                StudentNumber = b.Student.StudentNumber,
                BookTitle = b.Book.Name,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                StudentID = b.Student.ID,
            })
           .ToList();

            ViewBag.SearchName = searchName;

            return View(blackList);
        }

        public ActionResult ForgivePenalty(int id)// admin ve mod rollerimiz var toplu affetme ve tekil affetme işlemleri sadece admin tarafından sağlanmaktadır.
        {           
            Manager manager = Session["ManagerSession"] as Manager;

            if (manager == null || manager.ManagerRole_ID != 1)
            {
                TempData["mesaj"] = "Bu işlemi yapmaya yetkiniz yok. Admin girişi gereklidir.";
                return RedirectToAction("Index", "BlackList");
            }

            Borrow ceza = db.Borrows.FirstOrDefault(b => b.ID == id && b.Penalty > 0);

            if (ceza == null)
            {
                TempData["mesaj"] = "Affedilecek ceza bulunamadı ya da zaten affedilmiş.";
                return RedirectToAction("Index", "BlackList");
            }

            ceza.Penalty = 0;
            db.SaveChanges();

            TempData["mesaj"] = "Seçilen ceza affedildi.";
            return RedirectToAction("Index", "BlackList");
        }
        

        public ActionResult ForgiveAllPenalties()// admin ve mod rollerimiz var toplu affetme ve tekil affetme işlemleri sadece admin tarafından sağlanmaktadır.
        {
            Manager manager = Session["ManagerSession"] as Manager;

            if (manager == null || manager.ManagerRole_ID != 1)
            {
                TempData["mesaj"] = "Bu işlemi yapmaya yetkiniz yok. Admin girişi gereklidir.";
                return RedirectToAction("Index", "BlackList");
            }

            List<Borrow> cezalar = db.Borrows.Where(b => b.Penalty > 0).ToList();

            foreach (Borrow c in cezalar)
            {
                c.Penalty = 0;
            }

            db.SaveChanges();

            TempData["mesaj"] = "Tüm öğrencilerin cezaları başarıyla affedildi.";
            return RedirectToAction("Index", "BlackList");
        }
    }


}