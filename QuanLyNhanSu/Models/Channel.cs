using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhanSu.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> ConnectedUsers { get; set; }
    }
}