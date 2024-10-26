using Microsoft.AspNet.SignalR;
using QuanLyNhanSu.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Hubs;

namespace QuanLyNhanSu.Controllers
{
    public class VoiceChannelController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat(string username)
        {
            ViewBag.Username = username;
            return View();
        }

    }

}