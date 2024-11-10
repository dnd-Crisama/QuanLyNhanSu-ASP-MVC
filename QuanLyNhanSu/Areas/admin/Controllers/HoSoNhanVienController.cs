using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Web.Hosting;


namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class HoSoNhanVienController : AuthorController
    {
        // GET: admin/HoSoNhanVien
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /admin/QuanLyUser/
        public ActionResult Index()
        {
            var user = db.NhanViens
                 .Where(x => x.MaNhanVien != "admin" && x.TrangThai == true)
                 .ToList()
                 .OrderBy(m => {
                     int id;
                     return int.TryParse(m.MaNhanVien, out id) ? id : int.MaxValue;
                 })
                 .ToList();
            return View(user);
        }
        public ActionResult Detail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            return View(nhanVien);
        }
        public ActionResult XuatFile(string id)
        {
            var nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Profile");

                // Set up company header
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A1"].Value = "AVATAR";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add some space after company name
                worksheet.Cells["A2:E2"].Merge = true;

                // Load and position avatar image below company name, centered
                string avatarPath = HostingEnvironment.MapPath($"~/Content/images/{nhanVien.HinhAnh}");
                if (System.IO.File.Exists(avatarPath))
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(avatarPath);
                    var picture = worksheet.Drawings.AddPicture("Avatar", new MemoryStream(imageBytes));

                    // Position the image below company name (row 2) and center it
                    // The first two parameters are row and column offsets for the start position
                    // The last two parameters are row and column offsets for the end position
                    picture.SetPosition(2, 0, 2, 0);

                    // Make the image larger (150x150 pixels)
                    picture.SetSize(150, 150);
                }

                // Add space after the image
                worksheet.Row(3).Height = 115; // Adjust this value based on image size

                // Profile title - moved down to accommodate the larger image
                worksheet.Cells["A5:E5"].Merge = true;
                worksheet.Cells["A5"].Value = "THÔNG TIN NHÂN VIÊN";
                worksheet.Cells["A5"].Style.Font.Bold = true;
                worksheet.Cells["A5"].Style.Font.Size = 14;
                worksheet.Cells["A5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A5"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // Define sections and their data
                var sections = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "Thông tin cá nhân", new Dictionary<string, string>
                {
                    { "Họ và tên", nhanVien.HoTen },
                    { "Số CMND", nhanVien.CMND },
                    { "Ngày sinh", nhanVien.NgaySinh?.ToString("dd/MM/yyyy") },
                    { "Giới tính", nhanVien.GioiTinh == 1 ? "Nam" : nhanVien.GioiTinh == 0 ? "Nữ" : "Khác" },
                    { "Quê quán", nhanVien.QueQuan },
                    { "Dân tộc", nhanVien.DanToc }
                }
            },
            {
                "Thông tin liên hệ", new Dictionary<string, string>
                {
                    { "Số điện thoại", nhanVien.sdt_NhanVien },
                    { "Email cá nhân", nhanVien.Email },
                    { "Email công ty", $"{nhanVien.HoTen.ToLower().Replace(" ", "")}-{nhanVien.MaNhanVien}-{nhanVien.MaChuyenNganh}@gmail.com" }
                }
            },
            {
                "Thông tin công việc", new Dictionary<string, string>
                {
                    { "Mã nhân viên", $"{nhanVien.MaChuyenNganh}-{nhanVien.MaNhanVien}" },
                    { "Chuyên ngành", nhanVien.ChuyenNganh?.TenChuyenNganh },
                    { "Vị trí", nhanVien.ChucVuNhanVien?.TenChucVu },
                    { "Phòng ban", nhanVien.PhongBan?.TenPhongBan },
                    { "Trình độ học vấn", nhanVien.TrinhDoHocVan?.TenTrinhDo }
                }
            },
            {
                "Thông tin hợp đồng", new Dictionary<string, string>
                {
                    { "Mã hợp đồng", nhanVien.HopDong?.MaHopDong },
                    { "Loại hợp đồng", nhanVien.HopDong?.LoaiHopDong },
                    { "Ngày bắt đầu", nhanVien.HopDong?.NgayBatDau?.ToString("dd/MM/yyyy") },
                    { "Ngày kết thúc", nhanVien.HopDong?.NgayKetThuc?.ToString("dd/MM/yyyy") }
                }
            }
        };

                // Write sections - starting from row 7 to accommodate the larger image
                int currentRow = 7;
                foreach (var section in sections)
                {
                    // Section header
                    worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
                    worksheet.Cells[currentRow, 1].Value = section.Key;
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200));

                    currentRow++;

                    // Section data
                    foreach (var item in section.Value)
                    {
                        worksheet.Cells[currentRow, 1, currentRow, 2].Merge = true;
                        worksheet.Cells[currentRow, 1].Value = item.Key;
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

                        worksheet.Cells[currentRow, 3, currentRow, 5].Merge = true;
                        worksheet.Cells[currentRow, 3].Value = item.Value;

                        // Add subtle borders
                        var border = worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border;
                        border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        border.Bottom.Color.SetColor(Color.LightGray);

                        currentRow++;
                    }

                    currentRow++; // Add space between sections
                }

                // Format worksheet
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 20;

                // Add footer
                currentRow += 2;
                worksheet.Cells[$"A{currentRow}:E{currentRow}"].Merge = true;
                worksheet.Cells[$"A{currentRow}"].Value = $"Exported on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[$"A{currentRow}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Italic = true;

                // Generate file
                var excelData = package.GetAsByteArray();
                var fileName = $"Profile_{nhanVien.MaNhanVien}_{nhanVien.HoTen.Trim()}_{DateTime.Now:yyyyMMdd}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public ActionResult XuatTatCaFile()
        {
            var employees = db.NhanViens.Where(x => x.MaNhanVien != "admin" && x.TrangThai == true).ToList();

            using (var package = new ExcelPackage())
            {
                foreach (var nhanVien in employees)
                {
                    var worksheet = package.Workbook.Worksheets.Add($"Profile_{nhanVien.MaNhanVien}");

                    // Set up company header
                    worksheet.Cells["A1:E1"].Merge = true;
                    worksheet.Cells["A1"].Value = "AVATAR";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A2:E2"].Merge = true;

                    // Load avatar image if it exists
                    string avatarPath = HostingEnvironment.MapPath($"~/Content/images/{nhanVien.HinhAnh}");
                    if (System.IO.File.Exists(avatarPath))
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(avatarPath);
                        var picture = worksheet.Drawings.AddPicture("Avatar", new MemoryStream(imageBytes));
                        picture.SetPosition(2, 0, 2, 0);
                        picture.SetSize(150, 150);
                    }

                    worksheet.Row(3).Height = 115;

                    worksheet.Cells["A5:E5"].Merge = true;
                    worksheet.Cells["A5"].Value = "THÔNG TIN NHÂN VIÊN";
                    worksheet.Cells["A5"].Style.Font.Bold = true;
                    worksheet.Cells["A5"].Style.Font.Size = 14;
                    worksheet.Cells["A5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A5"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells["A5"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    var sections = new Dictionary<string, Dictionary<string, string>>
            {
                { "Thông tin cá nhân", new Dictionary<string, string> {
                    { "Họ và tên", nhanVien.HoTen },
                    { "Số CMND", nhanVien.CMND },
                    { "Ngày sinh", nhanVien.NgaySinh?.ToString("dd/MM/yyyy") },
                    { "Giới tính", nhanVien.GioiTinh == 1 ? "Nam" : nhanVien.GioiTinh == 0 ? "Nữ" : "Khác" },
                    { "Quê quán", nhanVien.QueQuan },
                    { "Dân tộc", nhanVien.DanToc }
                }},
                { "Thông tin liên hệ", new Dictionary<string, string> {
                    { "Số điện thoại", nhanVien.sdt_NhanVien },
                    { "Email cá nhân", nhanVien.Email },
                    { "Email công ty", $"{nhanVien.HoTen.ToLower().Replace(" ", "")}-{nhanVien.MaNhanVien}-{nhanVien.MaChuyenNganh}@gmail.com" }
                }},
                { "Thông tin công việc", new Dictionary<string, string> {
                    { "Mã nhân viên", $"{nhanVien.MaChuyenNganh}-{nhanVien.MaNhanVien}" },
                    { "Chuyên ngành", nhanVien.ChuyenNganh?.TenChuyenNganh },
                    { "Vị trí", nhanVien.ChucVuNhanVien?.TenChucVu },
                    { "Phòng ban", nhanVien.PhongBan?.TenPhongBan },
                    { "Trình độ học vấn", nhanVien.TrinhDoHocVan?.TenTrinhDo }
                }},
                { "Thông tin hợp đồng", new Dictionary<string, string> {
                    { "Mã hợp đồng", nhanVien.HopDong?.MaHopDong },
                    { "Loại hợp đồng", nhanVien.HopDong?.LoaiHopDong },
                    { "Ngày bắt đầu", nhanVien.HopDong?.NgayBatDau?.ToString("dd/MM/yyyy") },
                    { "Ngày kết thúc", nhanVien.HopDong?.NgayKetThuc?.ToString("dd/MM/yyyy") }
                }}
            };

                    int currentRow = 7;
                    foreach (var section in sections)
                    {
                        worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
                        worksheet.Cells[currentRow, 1].Value = section.Key;
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200));
                        currentRow++;

                        foreach (var item in section.Value)
                        {
                            worksheet.Cells[currentRow, 1, currentRow, 2].Merge = true;
                            worksheet.Cells[currentRow, 1].Value = item.Key;
                            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

                            worksheet.Cells[currentRow, 3, currentRow, 5].Merge = true;
                            worksheet.Cells[currentRow, 3].Value = item.Value;

                            var border = worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border;
                            border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            border.Bottom.Color.SetColor(Color.LightGray);

                            currentRow++;
                        }
                        currentRow++;
                    }

                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;

                    currentRow += 2;
                    worksheet.Cells[$"A{currentRow}:E{currentRow}"].Merge = true;
                    worksheet.Cells[$"A{currentRow}"].Value = $"Exported on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[$"A{currentRow}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Italic = true;
                }

                var excelData = package.GetAsByteArray();
                var fileName = $"AllEmployees_{DateTime.Now:yyyyMMdd}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}