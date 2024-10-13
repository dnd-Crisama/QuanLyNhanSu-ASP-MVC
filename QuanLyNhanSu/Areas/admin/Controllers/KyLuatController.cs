using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class KyLuatController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: /admin/KyLuat/
        public ActionResult Index()
        {
            var t = db.KyLuats.ToList();
            return View(t);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var nv = db.NhanViens.ToList();
            ViewBag.NhanVienList = nv;
            return View(new KyLuat());
        }

        [HttpPost]
        public ActionResult Create(KyLuat kl)
        {
            var existingKyLuat = db.KyLuats
                .Where(k => k.MaNhanVien == kl.MaNhanVien && k.ThangKiLuat == kl.ThangKiLuat)
                .FirstOrDefault();

            if (existingKyLuat != null)
            {
                ViewBag.ErrorKyLuatTrungMaNhanVien = "Nhân viên này đã có trong danh sách kỷ luật.";
                ViewBag.NhanVienList = db.NhanViens.ToList();
                return View(kl);
            }

            KyLuat ad = new KyLuat
            {
                MaNhanVien = kl.MaNhanVien,
                ThangKiLuat = kl.ThangKiLuat,
                LyDo = kl.LyDo,
                TienKyLuat = kl.TienKyLuat,
                TrangThai = false
            };

            try
            {
                db.KyLuats.Add(ad);
                db.SaveChanges();
            }
            catch
            {
                ViewBag.ErrorKyLuatTrungMaNhanVien = "Có lỗi xảy ra khi lưu thông tin kỷ luật.";
            }

            return Redirect("/admin/KyLuat");
        }

        [HttpGet]
        public ActionResult CapNhat(string id)
        {
            var kyLuat = db.KyLuats.Find(id);
            var nhanVien = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == kyLuat.MaNhanVien);
            ViewBag.HoTenNhanVien = nhanVien != null ? nhanVien.HoTen : "Không xác định";

            return View(kyLuat);
        }

        [HttpPost]
        public ActionResult CapNhat(KyLuat kl)
        {
            var existingKyLuat = db.KyLuats.Find(kl.MaNhanVien);

            if (existingKyLuat != null)
            {
                existingKyLuat.ThangKiLuat = kl.ThangKiLuat;
                existingKyLuat.LyDo = kl.LyDo;
                existingKyLuat.TienKyLuat = kl.TienKyLuat;
                existingKyLuat.TrangThai = kl.TrangThai;

                db.SaveChanges();
            }
            else
            {
                return HttpNotFound("Record not found");
            }

            return Redirect("/admin/KyLuat");
        }
    }
}