using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyHocVanController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: admin/QuanLyHocVan
        public ActionResult Index()
        {
            var hocVanList = db.TrinhDoHocVans.ToList();
            return View(hocVanList);
        }

        // GET: admin/QuanLyHocVan/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrinhDoHocVan hocVan)
        {
            if (ModelState.IsValid)
            {
                db.TrinhDoHocVans.Add(hocVan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hocVan);
        }

        // GET: admin/QuanLyHocVan/Edit/5
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var hocVan = db.TrinhDoHocVans.Find(id);
            if (hocVan == null)
            {
                return HttpNotFound("Record not found");
            }
            return View(hocVan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TrinhDoHocVan hocVan)
        {
            if (ModelState.IsValid)
            {
                var existingHocVan = db.TrinhDoHocVans.Find(hocVan.MaTrinhDoHocVan);
                if (existingHocVan != null)
                {
                    existingHocVan.TenTrinhDo = hocVan.TenTrinhDo;
                    existingHocVan.HeSoBac = hocVan.HeSoBac;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return HttpNotFound("Record not found");
            }
            return View(hocVan);
        }

        // GET: admin/QuanLyHocVan/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var hocVan = db.TrinhDoHocVans.Find(id);
            if (hocVan == null)
            {
                return HttpNotFound();
            }
            return View(hocVan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var hocVan = db.TrinhDoHocVans.Find(id);
            db.TrinhDoHocVans.Remove(hocVan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult getByTrinhDo(string id)
        {
            var td = db.NhanViens.Where(m=>m.MaTrinhDoHocVan == id).ToList();
            return View(td);
        }
    }
}


