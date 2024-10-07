using QuanLyNhanSu.Models;
using System.Linq;
using System;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class AdminController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        //
        // GET: /admin/Admin/
        public ActionResult Index()
        {
            var viewModel = new AdminDashboardViewModel();

            var luongs = db.Luongs.ToList();

            // Calculate total salary in memory
            var topEmployees = luongs
                .Select(item => new
                {
                    NameEmployee = item.NhanVien.HoTen, 
                    SumSalary = CalculateTotalSalary(item) 
                })
                .OrderByDescending(e => e.SumSalary)
                .Take(10) 
                .ToList();

            viewModel.TopEmployees = topEmployees.Select(x => new TopEmployee
            {
                NameEmployee = x.NameEmployee,
                SumSalary = (decimal)x.SumSalary
            }).ToList();
        // Employees by department
        viewModel.DepartmentEmployeeCounts = db.PhongBans
                .Select(pb => new DepartmentEmployeeCount
                {
                    DepartmentName = pb.TenPhongBan,
                    EmployeeCount = pb.NhanViens.Count(nv => nv.TrangThai)
                })
                .ToList();

            // Contract types
            viewModel.ContractTypeCounts = db.HopDongs
                .GroupBy(hd => hd.LoaiHopDong)
                .Select(g => new ContractTypeCount
                {
                    ContractType = g.Key,
                    Count = g.Count()
                })
                .Take(10)
                .ToList();

            // Employee roles
            viewModel.EmployeeRoleCounts = db.ChucVuNhanViens
                .Select(cv => new EmployeeRoleCount
                {
                    RoleName = cv.TenChucVu,
                    Count = cv.NhanViens.Count(nv => nv.TrangThai)
                })
                .ToList();

            // Other dashboard data
            viewModel.TotalEmployees = db.NhanViens.Count(nv => nv.TrangThai);
            viewModel.TotalDepartments = db.PhongBans.Count();
            viewModel.TotalSalaryRecords = db.Luongs.Count();
            viewModel.TotalSalaryAmount = CalculateTotalSalary();
            viewModel.TotalRewards = db.KhenThuongs.Count();

            return View(viewModel);
        }
        private double CalculateTotalSalary()
        {
            return db.Luongs.Sum(l =>
                (l.LuongToiThieu * (l.HeSoLuong ?? 0)) +
                (l.LuongToiThieu * (l.PhuCap ?? 0)) -
                (l.LuongToiThieu * ((l.BHXH ?? 0) + (l.BHYT ?? 0) + (l.BHTN ?? 0)) / 100) -
                (l.LuongToiThieu * (l.ThueThuNhap ?? 0) / 100)
            );
        }
        public double CalculateTotalSalary(Luong luong)
        {
            double totalSalary = 0;

            double luongcb = luong.LuongToiThieu * (luong.HeSoLuong ?? 0);

            double xh = (luong.BHXH ?? 0) * luong.LuongToiThieu / 100;
            double yt = (luong.BHYT ?? 0) * luong.LuongToiThieu / 100;
            double tn = (luong.BHTN ?? 0) * luong.LuongToiThieu / 100;

            double phucap = luong.LuongToiThieu * (luong.PhuCap ?? 0);
            double tienthue = luong.LuongToiThieu * (luong.ThueThuNhap ?? 0) / 100;

            totalSalary = luongcb - (xh + yt + tn) - tienthue + phucap;

            return totalSalary;
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