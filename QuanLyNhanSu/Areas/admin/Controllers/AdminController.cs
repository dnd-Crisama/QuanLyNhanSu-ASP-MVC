using QuanLyNhanSu.Models;
using System.Linq;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class AdminController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        //
        // GET: /admin/Admin/
        public ActionResult Index()
        {
            QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

            // Employee fluctuations over the last 6 months (new hires and removals)
            var lastSixMonths = DateTime.Now.AddMonths(-6);

            // New employees (hired within the last 6 months)
            var newEmployees = db.NhanViens
                .Where(nv => nv.TrangThai == true && nv.HopDong.NgayBatDau >= lastSixMonths)
                .GroupBy(nv => new { nv.HopDong.NgayBatDau.Value.Month, nv.HopDong.NgayBatDau.Value.Year })
                .Select(group => new { Month = group.Key.Month, Year = group.Key.Year, Count = group.Count() })
                .ToList();

            // Removed employees (those who have been terminated within the last 6 months)
            var removedEmployees = db.NhanViens
                .Where(nv => nv.TrangThai == false && nv.NgaySinh >= lastSixMonths)
                .GroupBy(nv => new { nv.NgaySinh.Value.Month, nv.NgaySinh.Value.Year })
                .Select(group => new { Month = group.Key.Month, Year = group.Key.Year, Count = group.Count() })
                .ToList();

            // Employee roles (Chức vụ) statistics
            var employeeRoles = db.NhanViens
                .GroupBy(nv => nv.ChucVuNhanVien.TenChucVu)
                .Select(group => new { Role = group.Key, Count = group.Count() })
                .ToList();

            // Employee distribution by department (for pie chart)
            var employeesByDepartment = db.NhanViens
                .GroupBy(nv => nv.PhongBan.TenPhongBan)
                .Select(group => new { Department = group.Key, Count = group.Count() })
                .ToList();

            // Contract statistics
            var contractStats = db.HopDongs
                .GroupBy(hd => hd.LoaiHopDong)
                .Select(group => new { ContractType = group.Key, Count = group.Count() })
                .ToList();

            ViewBag.NewEmployees = newEmployees;
            ViewBag.RemovedEmployees = removedEmployees;
            ViewBag.EmployeeRoles = employeeRoles;
            ViewBag.EmployeesByDepartment = employeesByDepartment;
            ViewBag.ContractStats = contractStats;
            return View();
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return Redirect("/");
        }

    }
}