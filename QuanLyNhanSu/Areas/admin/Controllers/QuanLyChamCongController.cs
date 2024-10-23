using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class QuanLyChamCongController : AuthorController
    {
        // GET: admin/ChamCong
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // View cho nhân viên nhập mã chấm công
        public ActionResult NhapMaChamCong()
        {
            return View();
        }

        // Xử lý sau khi nhân viên nhập mã chấm công
        [HttpPost]
        public ActionResult XacNhanChamCong(string maChamCong)
        {
            // Lấy mã chấm công từ DB
            var maChamCongDB = db.MaChamCongs.FirstOrDefault(m => m.MaChamCong1 == maChamCong && m.NgayTao == DateTime.Now.Date);

            if (maChamCongDB != null)
            {
                // Lấy thông tin nhân viên từ session (giả sử mã nhân viên được lưu trong session)
                string maNhanVien = (string)Session["MaNhanVien"];

                // Kiểm tra xem đã chấm công hôm nay chưa
                var daChamCong = db.BangChamCongs.FirstOrDefault(c => c.MaNhanVien == maNhanVien && c.NgayChamCong == DateTime.Now.Date);
                if (daChamCong == null)
                {
                    // Thêm bản ghi chấm công mới
                    BangChamCong chamCong = new BangChamCong
                    {
                        MaNhanVien = maNhanVien,
                        NgayChamCong = DateTime.Now.Date,
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

        // View hiển thị bảng chấm công cho admin, có tìm kiếm theo tháng, năm, mã nhân viên
        public ActionResult QuanLyChamCong(string maNhanVien = "", int thang = 0, int nam = 0)
        {
            ViewBag.CodeChamCong = TaoMaChamCong();
            var chamCongList = db.BangChamCongs.AsQueryable();

            if (!string.IsNullOrEmpty(maNhanVien))
            {
                chamCongList = chamCongList.Where(c => c.MaNhanVien == maNhanVien);
            }

            if (thang > 0)
            {
                chamCongList = chamCongList.Where(c => c.NgayChamCong.HasValue && c.NgayChamCong.Value.Month == thang);
            }

            if (nam > 0)
            {
                chamCongList = chamCongList.Where(c => c.NgayChamCong.HasValue && c.NgayChamCong.Value.Year == nam);
            }

            return View(chamCongList.OrderBy(c => c.NgayChamCong).ToList());
        }

        // Hàm để random mã chấm công cho mỗi ngày (có thể được gọi tự động hàng ngày)
        public string TaoMaChamCong()
        {
            var now = DateTime.Now.Day;
            MaChamCong codehientai = db.MaChamCongs.Where(m => m.NgayTao.Value.Day == now).SingleOrDefault();
            if (codehientai != null) 
            {
                return codehientai.MaChamCong1;
            }
            // Tạo mã chấm công random
            string maChamCong = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            // Lưu vào DB
            MaChamCong newMaChamCong = new MaChamCong
            {
                MaChamCong1 = maChamCong,
                NgayTao = DateTime.Now.Date
            };
            db.MaChamCongs.Add(newMaChamCong);
            db.SaveChanges();

            // Xóa mã chấm công của ngày hôm trước
            var maChamCongCu = db.MaChamCongs.Where(m => m.NgayTao.Value.Day < now);
            if (maChamCongCu != null)
            {
                db.MaChamCongs.RemoveRange(maChamCongCu);
                db.SaveChanges();
            }
            return newMaChamCong.MaChamCong1;
        }
    }
}