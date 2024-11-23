using QuanLyNhanSu.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace QuanLyNhanSu.Controllers
{
    public class NhanVienController : Controller
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /NhanVien/
        public ActionResult Index()
        {
            var id = Session["MaNhanVien"] as string;
            var chitiet = db.ChiTietLuongs.Where(n => n.MaNhanVien == id).OrderByDescending(n=>n.NgayNhanLuong.Month).ToList();
            return View(chitiet);
        }
        public ActionResult FooterPartial()
        {
            return PartialView("_FooterPartial");
        }
        public ActionResult Detail(string  id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return RedirectToAction("Index","Home");
            }

            return View(nhanVien);
        }
    }
}