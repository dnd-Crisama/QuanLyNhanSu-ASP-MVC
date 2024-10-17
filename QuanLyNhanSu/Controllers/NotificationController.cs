using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Controllers
{
    public class NotificationController : Controller
    {
        private QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: Notification/Index
        public ActionResult Index()
        {
            var notifications = db.Notifications.OrderByDescending(n => n.SentAt).ToList();
            
            return View(notifications);
        }

        // POST: Notification/SendNotification
        [HttpPost]
        public ActionResult SendNotification(string content ,string sender)
        {
            var notification = new Notification
            {
                Content = content,
                SentAt = DateTime.Now,
                SenderId = sender
            };

            db.Notifications.Add(notification);
            db.SaveChanges();
            ViewBag.Noti = DateTime.Now;

            return RedirectToAction("Index");
        }
    }
}