using QuanLyNhanSu.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class CaiDatController : Controller
    {
        private readonly QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        public CaiDatController()
        {
            String session = System.Web.HttpContext.Current.Session["MaNhanVien"] as String;
            if (System.Web.HttpContext.Current.Session["MaNhanVien"] == null || session != "admin")
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
        }

        // GET: admin/CaiDat
        public ActionResult Index()
        {
            var images = db.SlideImages.ToList();
            return View(images);
        }

        // POST: admin/CaiDat/Save (Add or Edit)
        [HttpPost]
        public ActionResult Save(SlideImage model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (model.id == 0)
                {
                    if (file != null && file.ContentLength > 0)
                    {

                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                        file.SaveAs(filePath);

                        model.src = fileName;                 
                    }

                    db.SlideImages.Add(model);
                }
                else 
                {
                    var image = db.SlideImages.Find(model.id);
                    if (image != null)
                    {
                        image.title = model.title;

                        if (file != null && file.ContentLength > 0)
                        {
                            string fileName = model.src;
                            string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                            file.SaveAs(filePath);
                           
                            image.src = fileName;
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Index", db.SlideImages.ToList());
        }

        // POST: admin/CaiDat/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var image = db.SlideImages.Find(id);
            if (image != null)
            {
                string filePath = Server.MapPath(image.src);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                db.SlideImages.Remove(image);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult FooterPartial()
        {
            return PartialView("_FooterPartial","Home");
        }
    }
}
