//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyNhanSu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KyLuat
    {
        public string MaNhanVien { get; set; }
        public string LyDo { get; set; }
        public System.DateTime ThangKiLuat { get; set; }
        public Nullable<int> TienKyLuat { get; set; }
        public Nullable<bool> TrangThai { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
    }
}
