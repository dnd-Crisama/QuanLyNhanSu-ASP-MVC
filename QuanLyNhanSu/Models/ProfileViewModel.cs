using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhanSu.Models
{
    public class ProfileViewModel
    {
        public NhanVien User { get; set; }
        public List<Post> Posts { get; set; }
        public List<Photo> Photos { get; set; }
    }
}