
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
    
public partial class TrinhDoHocVan
{

    public TrinhDoHocVan()
    {

        this.NhanViens = new HashSet<NhanVien>();

    }


    public string MaTrinhDoHocVan { get; set; }

    public string TenTrinhDo { get; set; }

    public Nullable<double> HeSoBac { get; set; }



    public virtual ICollection<NhanVien> NhanViens { get; set; }

}

}
