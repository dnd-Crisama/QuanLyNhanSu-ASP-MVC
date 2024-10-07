using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhanSu.Models
{
    public class AdminDashboardViewModel
    {
        public List<EmployeeFluctuation> EmployeeFluctuations { get; set; }
        public List<DepartmentEmployeeCount> DepartmentEmployeeCounts { get; set; }
        public List<ContractTypeCount> ContractTypeCounts { get; set; }
        public List<EmployeeRoleCount> EmployeeRoleCounts { get; set; }
        public List<TopEmployee> TopEmployees { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalSalaryRecords { get; set; }
        public double TotalSalaryAmount { get; set; }
        public int TotalRewards { get; set; }
    }
    public class TopEmployee
    {
        public string NameEmployee { get; set; }
        public decimal SumSalary { get; set; }
    }
    public class EmployeeFluctuation
    {
        public DateTime Date { get; set; }
        public int NewEmployees { get; set; }
        public int RemovedEmployees { get; set; }
    }

    public class DepartmentEmployeeCount
    {
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class ContractTypeCount
    {
        public string ContractType { get; set; }
        public int Count { get; set; }
    }

    public class EmployeeRoleCount
    {
        public string RoleName { get; set; }
        public int Count { get; set; }
    }
}