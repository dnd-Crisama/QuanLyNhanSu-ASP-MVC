using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhanSu.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public bool IsMuted { get; set; }
        public bool IsDeafened { get; set; }

        public int? CurrentChannelId { get; set; }
    }
}