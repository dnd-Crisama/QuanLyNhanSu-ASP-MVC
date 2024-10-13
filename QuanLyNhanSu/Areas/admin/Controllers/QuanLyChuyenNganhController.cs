using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyChuyenNganhController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: admin/QuanLyChuyenNganh
        public ActionResult Index()
        {
            var chuyenNganhList = db.ChuyenNganhs.ToList();
            return View(chuyenNganhList);
        }

        // GET: ChuyenNganh/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChuyenNganh chuyenNganh)
        {
            if (ModelState.IsValid)
            {
                db.ChuyenNganhs.Add(chuyenNganh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chuyenNganh);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var chuyenNganh = db.ChuyenNganhs.Find(id);
            if (chuyenNganh == null)
            {
                return HttpNotFound("Record not found");
            }
            return View(chuyenNganh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChuyenNganh cn)
        {
            if (ModelState.IsValid)
            {
                var existingChuyenNganh = db.ChuyenNganhs.Find(cn.MaChuyenNganh);
                if (existingChuyenNganh != null)
                {
                    existingChuyenNganh.TenChuyenNganh = cn.TenChuyenNganh;
                    db.SaveChanges();
                    return Redirect("/admin/QuanLyChuyenNganh");
                }
                return HttpNotFound("Record not found");
            }
            return View(cn);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            var chuyenNganh = db.ChuyenNganhs.SingleOrDefault(cn => cn.MaChuyenNganh == id);
            if (chuyenNganh == null)
            {
                return HttpNotFound();
            }
            return View(chuyenNganh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var chuyenNganh = db.ChuyenNganhs.SingleOrDefault(cn => cn.MaChuyenNganh == id);
            db.ChuyenNganhs.Remove(chuyenNganh);
            db.SaveChanges();
            return Redirect("/admin/QuanLyChuyenNganh");
        }

        public ActionResult FindEmployees(string id)
        {
            var employees = db.NhanViens.Where(nv => nv.MaChuyenNganh == id).ToList();
            var tencn = db.ChuyenNganhs.Find(id).TenChuyenNganh;
            ViewBag.tencn = tencn;
            return View(employees);
        }
        [HttpGet]
        public ActionResult EditChuyenNganhNV(String id)
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
        public ActionResult EditChuyenNganhNV(UserValidate upUser)
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
                    return Redirect("/admin/QuanLyChuyenNganh");

                
            }
            return View(upUser);

        }//end update
    }
}