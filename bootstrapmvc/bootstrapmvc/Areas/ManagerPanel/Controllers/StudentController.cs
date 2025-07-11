﻿using bootstrapmvc.Areas.ManagerPanel.Filters;
using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace bootstrapmvc.Areas.ManagerPanel.Controllers
{
    [ManagerLoginRequiredFilter]
    public class StudentController : Controller
    {
        Model1 db = new Model1();
        public ActionResult Index(string searchName)
        {
            List<Student> studentSearchName = db.Students.Where(x => x.IsDeleted == false).ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                studentSearchName = studentSearchName.Where(x =>(!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchName)) || (!string.IsNullOrEmpty(x.Surname) && x.Surname.Contains(searchName))).ToList();
            }

            ViewBag.SearchName = searchName;

            return View(studentSearchName);
        }
        public ActionResult _Index(string searchName)
        {
            List<Student> studentSearchName = db.Students.Where(x => x.IsDeleted == true).ToList();

            if (!string.IsNullOrEmpty(searchName))
            {
                studentSearchName = studentSearchName.Where(x => (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchName)) || (!string.IsNullOrEmpty(x.Surname) && x.Surname.Contains(searchName))).ToList();
            }

            ViewBag.SearchName = searchName;

            return View(studentSearchName);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Student model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Students.Add(model);
                    db.SaveChanges();
                    TempData["mesaj"] = "Öğrenci başarılı bir şekilde eklendi.";
                    return RedirectToAction("Index", "Student");

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
                Student c = db.Students.Find(id);
                if (c != null)
                {
                    return View(c);
                }
            }
            return RedirectToAction("Index", "Student");

        }
        [HttpPost]
        public ActionResult Edit(Student model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(model).Property(x => x.RegistrationDate).IsModified = false;
                    db.SaveChanges();
                    TempData["mesaj"] = "Öğrenci güncelleme başarılı";
                    return RedirectToAction("Index", "Student");
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
                Student c = db.Students.Find(id);
                if (c != null)
                {
                    c.IsDeleted = true;
                    c.IsActive = false;
                    db.SaveChanges();
                    TempData["mesaj"] = "Öğrenci silme işlemi başarılı";
                }
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult Back(int? id)
        {
            if (id != null)
            {
                Student c = db.Students.Find(id);
                if (c != null)
                {
                    c.IsDeleted = false;
                    c.IsActive = true;
                    db.SaveChanges();
                    TempData["mesaj"] = "Öğrenci aktif etme işlemi başarılı";
                }
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult ActivateAll()
        {
            List<Student> silinmisOgrenciler = db.Students.Where(c => c.IsDeleted == true).ToList();

            foreach (Student ogrenci in silinmisOgrenciler)
            {
                ogrenci.IsActive = true;
                ogrenci.IsDeleted = false;
            }

            db.SaveChanges();

            TempData["mesaj"] = "Tüm öğrenciler başarıyla aktifleştirildi.";
            return RedirectToAction("Index", "Student");
        }
        public ActionResult Details(int? id)
        {
            Student student = db.Students.Find(id);

            return View(student);
        }
    }
}