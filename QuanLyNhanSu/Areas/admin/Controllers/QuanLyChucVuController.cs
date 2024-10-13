using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyChucVuController : AuthorController
    {
        // GET: admin/QuanLyChucVu
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: ChucVu
        public ActionResult Index()
        {
            var chucVuList = db.ChucVuNhanViens.ToList();
            return View(chucVuList);
        }

        // GET: ChucVu/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChucVuNhanVien chucVu)
        {
            if (ModelState.IsValid)
            {
                db.ChucVuNhanViens.Add(chucVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chucVu);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var chucvu = db.ChucVuNhanViens.Find(id);
            if (chucvu == null)
            {
                return HttpNotFound("Record not found");
            }
            return View(chucvu);
        }

        // POST: ChucVu/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChucVuNhanVien cv)
        {
            if (ModelState.IsValid)
            {
                var existingChucVu = db.ChucVuNhanViens.Find(cv.MaChucVuNV);
                if (existingChucVu != null)
                {
                    existingChucVu.TenChucVu = cv.TenChucVu;
                    existingChucVu.HSPC = cv.HSPC;
                    db.SaveChanges();
                    return Redirect("/admin/QuanLyChucVu");
                }
                return HttpNotFound("Record not found");
            }
            return View(cv);
        }

        // GET: ChucVu/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var chucVu = db.ChucVuNhanViens.SingleOrDefault(cv => cv.MaChucVuNV == id);
            if (chucVu == null)
            {
                return HttpNotFound();
            }
            return View(chucVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var chucVu = db.ChucVuNhanViens.SingleOrDefault(cv => cv.MaChucVuNV == id);
            db.ChucVuNhanViens.Remove(chucVu);
            db.SaveChanges();
            return Redirect("/admin/QuanLyChucVu");
        }
        public ActionResult getByChucVu(string id)
        {
            var gcv = db.NhanViens.Where(m => m.MaChucVuNV == id).ToList();
            return View(gcv);
        }
        [HttpGet]
        public ActionResult EditChucVuNV(String id)
        {
            var user = db.NhanViens.Where(n => n.MaNhanVien == id).FirstOrDefault();
            UserValidate userVal = new UserValidate();

            userVal.MaNhanVien = user.MaNhanVien;
            userVal.HoTen = user.HoTen;
            userVal.MatKhau = user.MatKhau;
            userVal.GioiTinh = user.GioiTinh;

            userVal.MaChucVuNV = user.MaChucVuNV;
            userVal.QueQuan = user.QueQuan;
            userVal.HinhAnh = user.HinhAnh;
            userVal.DanToc = user.DanToc;
            userVal.sdt_NhanVien = user.sdt_NhanVien;
            userVal.MaHopDong = user.MaHopDong;

            userVal.NgaySinh = user.NgaySinh;
            userVal.TrangThai = user.TrangThai;
            userVal.MaChuyenNganh = user.MaChuyenNganh;
            userVal.MaTrinhDoHocVan = user.MaTrinhDoHocVan;
            userVal.MaPhongBan = user.MaPhongBan;

            userVal.CMND = user.CMND;
            userVal.Email = user.Email;
            userVal.Facebookk = user.Facebookk;
            userVal.Bio = user.Bio;
            userVal.XacNhanMatKhau = user.MatKhau;

            return View(userVal);
            //  return View(user);
        }
        [HttpPost]
        public ActionResult EditChucVuNV(UserValidate upUser)
        {
            upUser.XacNhanMatKhau = upUser.MatKhau;
            var us = db.NhanViens.Where(n => n.MaNhanVien == upUser.MaNhanVien).FirstOrDefault();

            //var us = db.NhanViens.Where(n => n.MaNhanVien == upUser.MaNhanVien).FirstOrDefault();
            if (us != null)
            {

                us.HoTen = upUser.HoTen;
                us.MatKhau = upUser.MatKhau;
                us.GioiTinh = upUser.GioiTinh;

                us.MaChucVuNV = upUser.MaChucVuNV;
                us.QueQuan = upUser.QueQuan;
                us.HinhAnh = upUser.HinhAnh;
                us.DanToc = upUser.DanToc;
                us.sdt_NhanVien = upUser.sdt_NhanVien;
                us.MaHopDong = upUser.MaHopDong;

                us.NgaySinh = upUser.NgaySinh;
                us.TrangThai = upUser.TrangThai;
                us.MaChuyenNganh = upUser.MaChuyenNganh;
                us.MaTrinhDoHocVan = upUser.MaTrinhDoHocVan;
                us.MaPhongBan = upUser.MaPhongBan;

                us.CMND = upUser.CMND;
                us.Email = upUser.Email;
                us.Facebookk = upUser.Facebookk;
                us.Bio = upUser.Bio;

                db.SaveChanges();
                return Redirect("/admin/QuanLyChucVu");


            }
            return View(upUser);
        }
    }
    }