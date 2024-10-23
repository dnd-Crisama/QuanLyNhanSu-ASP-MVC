using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Controllers
{
    public class ChamCongController : Controller
    {
        // GET: ChamCong
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: NhanVien/ChamCong/NhapMaChamCong
        public ActionResult NhapMaChamCong()
        {
            if (Session["MaNhanVien"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Xử lý sau khi nhân viên nhập mã chấm công
        [HttpPost]
        public ActionResult XacNhanChamCong(string maChamCong)
        {
            var datenow = DateTime.Now;
            // Lấy mã chấm công từ DB
            var maChamCongDB = db.MaChamCongs.FirstOrDefault(m => m.MaChamCong1 == maChamCong && m.NgayTao.Value.Day == datenow.Day && m.NgayTao.Value.Month == datenow.Month && m.NgayTao.Value.Year == datenow.Year);

            if (maChamCongDB != null)
            {
                // Lấy thông tin nhân viên từ session (giả sử mã nhân viên được lưu trong session)
                string maNhanVien = (string)Session["MaNhanVien"];

                // Kiểm tra xem đã chấm công hôm nay chưa
                var daChamCong = db.BangChamCongs.FirstOrDefault(c => c.MaNhanVien == maNhanVien && c.NgayChamCong.Value.Day == datenow.Day && c.NgayChamCong.Value.Month == datenow.Month && c.NgayChamCong.Value.Year == datenow.Year);
                if (daChamCong == null)
                {
                    // Thêm bản ghi chấm công mới
                    BangChamCong chamCong = new BangChamCong
                    {
                        MaNhanVien = maNhanVien,
                        NgayChamCong = DateTime.Now,
                        GhiChu = "Chấm công thành công"
                    };

                    db.BangChamCongs.Add(chamCong);
                    db.SaveChanges();
                    TempData["Success"] = "Chấm công thành công!";
                }
                else
                {
                    TempData["Error"] = "Bạn đã chấm công hôm nay rồi!";
                }
            }
            else
            {
                TempData["Error"] = "Mã chấm công không chính xác hoặc đã hết hạn!";
            }

            return RedirectToAction("NhapMaChamCong");
        }
    }
}