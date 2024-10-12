using DocumentFormat.OpenXml.Office2010.Excel;
using QuanLyNhanSu.Models;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class KhenThuongController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /admin/KhenThuong/
        public ActionResult Index()
        {
            var t = db.KhenThuongs.ToList();
            return View(t);
        }
        [HttpGet]
        public ActionResult khen()
        {
            var nv = db.NhanViens.ToList();

            return View(new KhenThuong());
        }
        [HttpPost]
        public ActionResult khen(KhenThuong kt)
        {
            //var ct = db.ChiTietLuongs.Where(n => n.MaNhanVien == kt.MaNhanVien).FirstOrDefault();

            var existingKhenThuong = db.KhenThuongs
    .Where(k => k.MaNhanVien == kt.MaNhanVien)
    .FirstOrDefault();

            if (existingKhenThuong != null)
            {
                ViewBag.ErrorKhenThuongTrungMaNhanVien = "Nhân viên này đã có trong danh sách khen thưởng.";
                var nhanvien = db.NhanViens.Where(n => n.MaNhanVien != "admin" && n.KhenThuong == null).ToList();
                return View(kt); 
            }
            KhenThuong ad = new KhenThuong();
            ad.MaNhanVien = kt.MaNhanVien;
            ad.ThangThuong = kt.ThangThuong;
            ad.TienThuong = kt.TienThuong;
            ad.LyDo = kt.LyDo;
            ad.TrangThai = false;
            try
            {
                db.KhenThuongs.Add(ad);
                db.SaveChanges();
            }
            catch
            {
                ViewBag.ErrorKhenThuongTrungMaNhanVien = "Nhân viên đã có trong danh sách khen thưởng!";
            }
            return Redirect("/admin/KhenThuong");
        }
        [HttpGet]
        public ActionResult CapNhat(string id)
        {
            var khenThuong = db.KhenThuongs.Find(id);

            return View(khenThuong);
        }
        public ActionResult CapNhat(KhenThuong kt)
        {

            KhenThuong ad = new KhenThuong();
            ad.MaNhanVien = kt.MaNhanVien;
            ad.ThangThuong = kt.ThangThuong;
            ad.TienThuong = kt.TienThuong;
            ad.LyDo = kt.LyDo;
            ad.TrangThai = false;

            db.SaveChanges();
            return Redirect("/admin/KhenThuong");
        }
    }
}