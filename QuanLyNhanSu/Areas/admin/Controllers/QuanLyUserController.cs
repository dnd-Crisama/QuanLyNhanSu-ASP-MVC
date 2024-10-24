﻿using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

//using cExcel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyUserController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /admin/QuanLyUser/
        public ActionResult Index(bool? showDisable = false)
        {
            ViewBag.showDisable = showDisable;
            var user = db.NhanViens
                .Where(x => x.MaNhanVien != "admin" && (showDisable == true || x.TrangThai == true))
                .ToList()
                .OrderBy(m => {
                    int id;
                    return int.TryParse(m.MaNhanVien, out id) ? id : int.MaxValue;
                })
                .ToList();
            return View(user);
        }
        [HttpGet]
        public ActionResult Xoa(string id)
        {
            var user = db.NhanViens.SingleOrDefault(x => x.MaNhanVien == id);
            if (user == null)
            {
                return HttpNotFound("Employee not found");
            }

            return View("XoaUser",user);  // Render the XacNhanXoa.cshtml view
        }

        [HttpPost]
        public ActionResult XoaUser(String id,string lydo)
        {
            var employee = db.NhanViens.SingleOrDefault(x => x.MaNhanVien == id);

            var a = db.NhanViens.Where(x => x.MaNhanVien == id).SingleOrDefault();
            var hd = db.HopDongs.Where(x => x.MaHopDong == id).SingleOrDefault();
            var luong = db.Luongs.Where(x => x.MaNhanVien == id).SingleOrDefault();
            var ctLuong = db.ChiTietLuongs.Where(x => x.MaNhanVien == id).ToList();

            // bang mot cach nao do ma tao khong dung remove duoc nen execute bang sql command
            string sql = "INSERT INTO ThoiViecs (MaNhanVien, TenNhanVien, NgayThoiViec, Lydo) VALUES (@MaNhanVien, @TenNhanVien, @NgayThoiViec, @Lydo)";
            db.Database.ExecuteSqlCommand(sql, new SqlParameter("@MaNhanVien", a.MaNhanVien),
                new SqlParameter("@TenNhanVien", a.HoTen),
                new SqlParameter("@NgayThoiViec", DateTime.Now),
                new SqlParameter("@Lydo", lydo));

           /*
            foreach (var item in ctLuong)
            {
                db.ChiTietLuongs.Remove(item);
            }

            db.Luongs.Remove(luong);
            db.NhanViens.Remove(a);
            */
            db.HopDongs.Remove(hd);

         
            db.SaveChanges();
            return Redirect("~/admin/QuanLyUser");
        }
        [HttpGet]
        public ActionResult UpdateUser(String id)
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
        public ActionResult UpdateUser(UserValidate upUser)
        {
            upUser.XacNhanMatKhau = upUser.MatKhau;
            var us = db.NhanViens.Where(n => n.MaNhanVien == upUser.MaNhanVien).FirstOrDefault();

            if (ModelState.IsValid)
            {
                //var us = db.NhanViens.Where(n => n.MaNhanVien == upUser.MaNhanVien).FirstOrDefault();
                if (us != null)
                {

                    CapNhatTrinhDoHocVan capNhat = new CapNhatTrinhDoHocVan();
                    capNhat.MaNhanVien = upUser.MaNhanVien;
                    capNhat.NgayCapNhat = DateTime.Now.Date;
                    capNhat.MaTrinhDoTruoc = us.MaTrinhDoHocVan;
                    capNhat.MaTrinhDoCapNhat = upUser.MaTrinhDoHocVan;

                    us.MaNhanVien = upUser.MaNhanVien;
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

                    var trinhdo = db.TrinhDoHocVans.Where(n => n.MaTrinhDoHocVan.Equals(us.MaTrinhDoHocVan)).FirstOrDefault();

                    var luong = db.Luongs.Where(n => n.MaNhanVien.Equals(us.MaNhanVien)).FirstOrDefault();

                    if (trinhdo.HeSoBac != null)
                    {
                        luong.HeSoLuong = luong.HeSoLuong < (double)trinhdo.HeSoBac ? (double)trinhdo.HeSoBac : luong.HeSoLuong;
                    }
                    else
                    { luong.HeSoLuong = 1; }



                    db.CapNhatTrinhDoHocVans.Add(capNhat);

                    db.SaveChanges();
                    return Redirect("/admin/QuanLyUser");

                }
            }
            return View(upUser);

        }//end update

        [HttpGet]

        public ActionResult ThemUser()
        {
            var lastEmployee = db.NhanViens
    .Where(x => x.MaNhanVien != "Admin") 
    .OrderByDescending(x => x.MaNhanVien.Length)
    .ThenByDescending(x => x.MaNhanVien) 
    .FirstOrDefault();
            if (lastEmployee != null)
            {
                var numericPart = int.Parse(lastEmployee.MaNhanVien);
                string nextMaNhanVien = (numericPart + 1).ToString();
                ViewBag.NextMaNhanVien = nextMaNhanVien;
            }
            else
            {
                ViewBag.NextMaNhanVien = null;
            }
            var chucvu = db.ChucVuNhanViens.ToList();
            var phongban = db.PhongBans.ToList();
            var hopdong = db.HopDongs.ToList();
            var chuyennganh = db.ChuyenNganhs.ToList();
            var trinhdo = db.TrinhDoHocVans.ToList();
            List<ChucVuNhanVien> list = chucvu;

            return View(new UserValidate());
        }


        [HttpPost]
        public ActionResult ThemUser(UserValidate nv)
        {

            nv.XacNhanMatKhau = nv.MatKhau;
            if (ModelState.IsValid)
            {
                ViewBag.err = String.Empty;
                var checkMaNhanVien = db.NhanViens.Any(x => x.MaNhanVien == nv.MaNhanVien);

                if (checkMaNhanVien)
                {
                    ViewBag.err = "tài khoản đã tồn tại";
                    //ModelState.AddModelError("MaNhanVien", "Mã tài khoản đã tồn tại");
                    return View(nv);
                }
                else
                {
                    Luong luong = new Luong();
                    HopDong hd = new HopDong();
                    NhanVien nvAdd = new NhanVien();
                    nvAdd.MaNhanVien = nv.MaNhanVien;
                    nvAdd.MatKhau = nv.MatKhau;
                    nvAdd.HoTen = nv.HoTen;
                    nvAdd.NgaySinh = nv.NgaySinh;
                    nvAdd.QueQuan = nv.QueQuan;
                    nvAdd.GioiTinh = nv.GioiTinh;
                    nvAdd.DanToc = nv.DanToc;
                    nvAdd.MaChucVuNV = nv.MaChucVuNV;
                    nvAdd.MaPhongBan = nv.MaPhongBan;
                    nvAdd.MaChuyenNganh = nv.MaChuyenNganh;
                    nvAdd.MaTrinhDoHocVan = nv.MaTrinhDoHocVan;
                    nvAdd.MaHopDong = nv.MaNhanVien;
                    nvAdd.TrangThai = true;
                    nvAdd.HinhAnh = "icon.jpg";

                    //add hop dong
                    hd.MaHopDong = nv.MaNhanVien;
                    hd.NgayBatDau = DateTime.Now.Date;

                    //tao bang luong
                    luong.MaNhanVien = nv.MaNhanVien;
                    luong.LuongToiThieu = 1150000;
                    luong.BHXH = 8;
                    luong.BHYT = 1.5;
                    luong.BHTN = 1;
                    var trinhdo = db.TrinhDoHocVans.Where(n => n.MaTrinhDoHocVan.Equals(nv.MaTrinhDoHocVan)).FirstOrDefault();
                    var chucvu = db.ChucVuNhanViens.Where(n => n.MaChucVuNV.Equals(nv.MaChucVuNV)).SingleOrDefault();

                    if (trinhdo.MaTrinhDoHocVan.Equals(nv.MaTrinhDoHocVan))
                    {
                        luong.HeSoLuong = (double)trinhdo.HeSoBac;
                    }


                    if (chucvu.MaChucVuNV.Equals(nv.MaChucVuNV))
                    {
                        if (chucvu.HSPC != null)
                        {
                            luong.PhuCap = (double)chucvu.HSPC;
                        }
                        else
                        { luong.PhuCap = 0; }
                    }



                    // tmp.Image = "~/Content/images/icon.jpg";
                    db.NhanViens.Add(nvAdd);
                    db.HopDongs.Add(hd);

                    db.Luongs.Add(luong);
                    // @ViewBag.add = "Đăng ký thành công";
                    db.SaveChanges();
                    //xác thực tài khoản trong ứng dụng
                    FormsAuthentication.SetAuthCookie(nvAdd.MaNhanVien, false);
                    //trả về trang quản lý

                    return Redirect("/admin/QuanLyUser");
                }
            }
            else
            {

                return View(nv);
            }
        }//end add nhan vien

        public ActionResult QuaTrinhCongTac(String id)
        {
            var ds = db.LuanChuyenNhanViens.Where(n => n.MaNhanVien == id).ToList();
            return View(ds);
        }

        public ActionResult XuatFileExel()
        {

            var ds = db.NhanViens.Where(n => n.MaNhanVien != "admin" && n.MaHopDong != null).ToList();
            var phong = db.PhongBans.ToList();
            var gv = new GridView();
            //===================================================
            DataTable dt = new DataTable();
            //Add Datacolumn
            DataColumn workCol = dt.Columns.Add("Họ tên", typeof(String));

            dt.Columns.Add("Phòng ban", typeof(String));
            dt.Columns.Add("Chức vụ", typeof(String));
            dt.Columns.Add("Học vấn", typeof(String));
            dt.Columns.Add("Chuyên ngành", typeof(String));

            //Add in the datarow


            foreach (var item in ds)
            {
                DataRow newRow = dt.NewRow();
                newRow["Họ tên"] = item.HoTen;
                newRow["Phòng ban"] = item.PhongBan.TenPhongBan;
                newRow["Chức vụ"] = item.ChucVuNhanVien.TenChucVu;
                newRow["Học vấn"] = item.TrinhDoHocVan.TenTrinhDo;
                newRow["Chuyên ngành"] = item.ChuyenNganh.TenChuyenNganh;

                dt.Rows.Add(newRow);
            }

            //====================================================
            gv.DataSource = dt;
            // gv.DataSource = ds;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment; filename=danh-sach.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Redirect("/admin/QuanLyUser");
        }

        public ActionResult QuaTrinhHoc(String id)
        {
            var ht = db.CapNhatTrinhDoHocVans.Where(n => n.MaNhanVien == id).ToList();
            return View(ht);
        }
        public ActionResult NgungHoatDong(string id)
        {
            var obj = db.NhanViens.SingleOrDefault(n => n.MaNhanVien == id);

            if (obj != null)
            {
                obj.TrangThai = false;
                db.SaveChanges();
            }
            return Redirect("/admin/QuanLyUser");
        }
        public ActionResult ThoiViec(bool? thoiviec = false)
        {
            ViewBag.ThoiViec = thoiviec;
            if(thoiviec == true)
            {
                string sql = "SELECT * FROM THOIVIECs"; 
                var data = db.Database.SqlQuery<ThoiViec>(sql).ToList();

                return View(data);
            }
            else
            {
                var data = db.NhanViens
                .Where(x => x.MaNhanVien != "admin" && x.TrangThai == true)
                .ToList()
                .OrderBy(m => {
                    int id;
                    return int.TryParse(m.MaNhanVien, out id) ? id : int.MaxValue;
                })
                .ToList();
                return View("Index",data);
            }   
           
        }
        public ActionResult XoaThoiViec (string id ,string name)
        {
            string sql = "DELETE FROM THOIVIECs WHERE MaNhanVien = @p0 AND TenNhanVien = @p1";
            db.Database.ExecuteSqlCommand(sql, id, name);
            return RedirectToAction("ThoiViec");
        }

    }   //end lass
}