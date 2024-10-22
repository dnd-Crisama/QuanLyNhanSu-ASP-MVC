using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyLuongController : AuthorController
    //     public class QuanLyLuongController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /admin/QuanLyLuong/
        public ActionResult Index()
        {
            var list = db.Luongs.ToList();
            return View(list);
        }
        [HttpGet]
        public ActionResult SuaBangLuong(String id)
        {
            var luong = db.Luongs.Where(n => n.MaNhanVien == id).SingleOrDefault();
            return View(luong);
        }
        [HttpPost]
        public ActionResult SuaBangLuong(Luong luong, CapNhatLuong up)
        {
            var l = db.Luongs.Where(n => n.MaNhanVien == luong.MaNhanVien).FirstOrDefault();
            if (l != null)
            {
                //  l.MaNhanVien = luong.MaNhanVien;
                if (int.Parse(up.LuongSauCapNhat.ToString()) != 0)
                {
                    l.LuongToiThieu = up.LuongSauCapNhat;
                }

                l.BHXH = luong.BHXH == null ? 0 : luong.BHXH;
                l.BHYT = luong.BHYT == null ? 0 : luong.BHYT;
                l.BHTN = luong.BHTN == null ? 0 : luong.BHTN;
                //   l.PhuCap = luong.PhuCap;
                l.ThueThuNhap = luong.ThueThuNhap;
                l.HeSoLuong = luong.HeSoLuong;

                //tao table luu lai moi lan cap nhat luong
                CapNhatLuong capNhat = new CapNhatLuong();
                capNhat.NgayCapNhat = DateTime.Now.Date;
                capNhat.MaNhanVien = luong.MaNhanVien;
                capNhat.LuongHienTai = luong.LuongToiThieu;
                capNhat.LuongSauCapNhat = up.LuongSauCapNhat;
                capNhat.BHXH = luong.BHXH;
                capNhat.BHYT = luong.BHYT;
                capNhat.BHTN = luong.BHTN;
                //  capNhat.PhuCap = luong.PhuCap;
                capNhat.ThueThuNhap = luong.ThueThuNhap;
                capNhat.HeSoLuong = luong.HeSoLuong;

                db.CapNhatLuongs.Add(capNhat);
                db.SaveChanges();
            }

            return Redirect("/admin/QuanLyLuong");
        }
        //end update lương

        public ActionResult ThanhToanLuong()
        {
            var luong = db.Luongs.ToList();
            List<string> processedEmployees = new List<string>(); // Store names of employees whose salary has already been processed.

            DateTime now = DateTime.Now;
            string currentMonthYear = "T" + now.Month.ToString() + "-" + now.Year.ToString();

            foreach (var item in luong)
            {
                ChiTietLuong ct = new ChiTietLuong
                {
                    MaChiTietBangLuong = currentMonthYear,
                    MaNhanVien = item.MaNhanVien
                };

                // Check if a salary detail already exists for the current month.
                var ctl = db.ChiTietLuongs
                    .Where(n => n.MaNhanVien == ct.MaNhanVien && n.MaChiTietBangLuong == currentMonthYear)
                    .FirstOrDefault();

                if (ctl != null)
                {
                    // Add the employee's name to the list if their salary for this month is already processed.
                    string ten = db.NhanViens.FirstOrDefault(m => m.MaNhanVien == item.MaNhanVien)?.HoTen;
                    if (!string.IsNullOrEmpty(ten))
                    {
                        processedEmployees.Add(ten);
                    }
                    continue; // Skip processing salary for this employee.
                }

                // Proceed with salary calculation if not already processed.
                double tienthue = 0, phucap = 0, tong = 0;
                item.HeSoLuong = item.HeSoLuong ?? 0;
                ct.LuongCoBan = item.LuongToiThieu * (double)item.HeSoLuong;

                item.BHXH = item.BHXH ?? 0;
                ct.BHXH = item.BHXH * item.LuongToiThieu / 100;

                item.BHYT = item.BHYT ?? 0;
                ct.BHYT = item.BHYT * item.LuongToiThieu / 100;

                item.BHTN = item.BHTN ?? 0;
                ct.BHTN = item.BHTN * item.LuongToiThieu / 100;

                item.PhuCap = item.PhuCap ?? 0;
                phucap = item.LuongToiThieu * (double)item.PhuCap;
                ct.PhuCap = phucap;

                item.ThueThuNhap = item.ThueThuNhap ?? 0;
                tienthue = item.LuongToiThieu * (double)item.ThueThuNhap / 100;
                ct.ThueThuNhap = tienthue;

                ct.NgayNhanLuong = now.Date;
                ct.TienThuong = db.KhenThuongs
                    .Where(m => m.MaNhanVien == ct.MaNhanVien && m.TrangThai != true)
                    .Select(m => m.TienThuong)
                    .FirstOrDefault() ?? 0;

                ct.TienPhat = db.KyLuats
                    .Where(x => x.MaNhanVien == ct.MaNhanVien && x.TrangThai != true)
                    .Select(x => x.TienKyLuat)
                    .FirstOrDefault() ?? 0;

                var ngaycong = (db.BangChamCongs.Where(x => x.MaNhanVien == ctl.MaNhanVien).Count()) * 0.1;

                tong = ct.LuongCoBan - (double)(ct.BHXH + ct.BHYT + ct.BHTN) - (double)ct.ThueThuNhap + (double)ct.PhuCap + (double)ct.TienThuong - (double)ct.TienPhat;
                ct.TongTienLuong = (tong*ngaycong).ToString();

                db.ChiTietLuongs.Add(ct);
                var khenthuong = db.KhenThuongs.FirstOrDefault(m => m.MaNhanVien == ct.MaNhanVien && m.TrangThai != true);
                if (khenthuong != null)
                {
                    khenthuong.TrangThai = true; 
                }

                var kyluat = db.KyLuats.FirstOrDefault(x => x.MaNhanVien == ct.MaNhanVien && x.TrangThai != true);
                if (kyluat != null)
                {
                    kyluat.TrangThai = true; 
                }
                TempData["ok"] = "Thanh toán thành công";
                db.SaveChanges();
            }

            // Store the list of processed employees in ViewBag to display in the view.
            TempData["ChiTietLuongErrorNOTNULL"] = processedEmployees;
            return Redirect("/admin/QuanLyLuong");
        }


        public ActionResult ThanhToanMotNhanVien(string id)
        {
            var nv = db.NhanViens.FirstOrDefault(n => n.MaNhanVien == id);
            if (nv != null)
            {
                DateTime now = DateTime.Now;
                string currentMonthYear = "T" + now.Month.ToString() + "-" + now.Year.ToString();

                // Check if salary details already exist for this employee for the current month.
                var ctl = db.ChiTietLuongs
                    .Where(n => n.MaNhanVien == id && n.MaChiTietBangLuong == currentMonthYear)
                    .FirstOrDefault();

                if (ctl != null)
                {
                    TempData["SINGLEChiTietLuongErrorNOTNULL"] = "Nhân viên đã được thanh toán lương cho tháng này.";
                    return Redirect("/admin/QuanLyLuong");
                }

                // Proceed with salary calculation if not already processed.
                var luongthang = db.Luongs.FirstOrDefault(n => n.MaNhanVien == id);
                ChiTietLuong ct = new ChiTietLuong
                {
                    MaChiTietBangLuong = currentMonthYear,
                    MaNhanVien = luongthang.MaNhanVien,
                    NgayNhanLuong = now.Date
                };
                double tienthue = 0, tong = 0, phucap = 0;

                var khenthuong = db.KhenThuongs.Where(m => m.MaNhanVien == id && m.TrangThai != true).FirstOrDefault();
                var kyluat = db.KyLuats.Where(x => x.MaNhanVien == id && x.TrangThai != true).FirstOrDefault();

                var ngaycong = (db.BangChamCongs.Where(x => x.MaNhanVien == id).Count()) * 0.1;

                ct.MaChiTietBangLuong = "T" + now.Month.ToString() + "-" + now.Year.ToString();
                ct.MaNhanVien = luongthang.MaNhanVien;

                ct.LuongCoBan = luongthang.LuongToiThieu * (double)luongthang.HeSoLuong;

                luongthang.BHXH = luongthang.BHXH == null ? 0 : luongthang.BHXH;
                ct.BHXH = luongthang.BHXH * luongthang.LuongToiThieu / 100;

                luongthang.BHYT = luongthang.BHYT == null ? 0 : luongthang.BHYT;
                ct.BHYT = luongthang.BHYT * luongthang.LuongToiThieu / 100;

                luongthang.BHTN = luongthang.BHTN == null ? 0 : luongthang.BHTN;
                ct.BHTN = luongthang.BHTN * luongthang.LuongToiThieu / 100;

                luongthang.PhuCap = luongthang.PhuCap == null ? 0 : luongthang.PhuCap;
                phucap = luongthang.LuongToiThieu * (double)luongthang.PhuCap;
                ct.PhuCap = phucap;


                luongthang.ThueThuNhap = luongthang.ThueThuNhap == null ? 0 : luongthang.ThueThuNhap;
                tienthue = (double)luongthang.LuongToiThieu * (double)luongthang.ThueThuNhap / 100;
                ct.ThueThuNhap = (double)tienthue;
                ct.NgayNhanLuong = DateTime.Now.Date;
                ct.TienThuong = khenthuong?.TienThuong ?? 0;
                ct.TienPhat = kyluat?.TienKyLuat ?? 0;
                
                tong = tong + ct.LuongCoBan - (double)(ct.BHXH + ct.BHYT + ct.BHTN) - (double)ct.ThueThuNhap + (double)ct.PhuCap +(double)ct.TienThuong - (double)ct.TienPhat;
                ct.TongTienLuong = (tong*ngaycong).ToString();
                if (ctl == null)
                {
                    TempData["ok"] = "Thanh toán thành công";
                    db.ChiTietLuongs.Add(ct);
                }
                if (khenthuong != null)
                {
                    khenthuong.TrangThai = true;
                }              
                if(kyluat != null)
                {
                    kyluat.TrangThai = true;
                }              
                db.SaveChanges();

            }
            return Redirect("/admin/QuanLyLuong");
        }

        public ActionResult DanhSachNhanLuong()
        {
            var list = db.ChiTietLuongs.ToList();
            return View(list);
        }
        public ActionResult XuatFileLuong(String id)
        {
            //var l = db.ChiTietLuongs.Where(n => n.MaChiTietBangLuong == id).ToList();
            var ds = db.ChiTietLuongs.ToList();
            //===================================================
            DataTable dt = new DataTable();
            //Add Datacolumn
            DataColumn workCol = dt.Columns.Add("Mã nhân viên", typeof(String));
            dt.Columns.Add("Lương cơ bản", typeof(String));
            dt.Columns.Add("BHXH", typeof(String));
            dt.Columns.Add("Phụ cấp", typeof(String));
            dt.Columns.Add("Thuế thu nhập", typeof(String));
            dt.Columns.Add("Ngày nhận lương", typeof(String));
            dt.Columns.Add("Thực lãnh", typeof(String));

            //Add in the datarow


            foreach (var item in ds)
            {
                DataRow newRow = dt.NewRow();
                newRow["Mã nhân viên"] = item.MaNhanVien;
                newRow["Lương cơ bản"] = item.LuongCoBan;
                newRow["BHXH"] = item.BHXH;
                newRow["Phụ cấp"] = item.PhuCap;
                newRow["Thuế thu nhập"] = item.ThueThuNhap;
                newRow["Ngày nhận lương"] = item.NgayNhanLuong;
                newRow["Thực lãnh"] = item.TongTienLuong;


                dt.Rows.Add(newRow);
            }

            //====================================================
            var gv = new GridView();
            //gv.DataSource = ds;
            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment; filename=danh-sach-luong.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Redirect("/admin/QuanLyLuong");
        }


        public ActionResult QuaTrinhTangLuong(String id)
        {
            var tangluong = db.CapNhatLuongs.Where(n => n.MaNhanVien == id).ToList();
            if (tangluong != null)
            {
                return View(tangluong);
            }
            return Redirect("/admin/QuanLyLuong");
        }// EndEv
    }   //end class
}
