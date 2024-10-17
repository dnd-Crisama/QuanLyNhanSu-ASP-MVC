using DocumentFormat.OpenXml.Spreadsheet;
using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using QuanLyNhanSu.Hubs;
using DocumentFormat.OpenXml.Wordprocessing;
using ClosedXML.Excel;


namespace QuanLyNhanSu.Controllers
{
    public class MessageController : Controller
    {
        public QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: Message/Index
        public ActionResult Index(int? conversationId)
        {
            Conversation conversation;
            var userId = (string)Session["MaNhanVien"];


            if (conversationId == null)
            {
                // Get the global conversation
                conversation = db.Conversations.FirstOrDefault(c => c.IsGlobal == true);
                conversationId = conversation.Id;
               
                if (conversation == null)
                {
                    return HttpNotFound("Global conversation not found.");
                }
            }
            else
            {
                // Get the conversation based on the provided ID
                conversation = db.Conversations.Find(conversationId);
                var p1 = conversation.Participant1Id;
                var p2 = conversation.Participant2Id;

                if(conversation.Participant1Id == userId)
                {
                    ViewBag.otherUserNamePrivately = db.NhanViens.FirstOrDefault(m=>m.MaNhanVien == p2).HoTen;
                }
                else
                {
                    ViewBag.otherUserNamePrivately = db.NhanViens.FirstOrDefault(m => m.MaNhanVien == p1).HoTen;
                }
                if (conversation == null)
                {
                    return HttpNotFound("Conversation not found.");
                }
                if (conversation.IsGlobal == false && conversation.Participant1Id != userId && conversation.Participant2Id != userId)
                {
                    return RedirectToAction("Index");
                }
            }

            if (db.Conversations.Where(m => (m.IsGlobal == false) && (m.Participant1Id == userId || m.Participant2Id == userId)).ToList() == null)
            {
                return RedirectToAction("Index", new { conversationId });
            }

            var messages = db.Messages.Where(m => m.ConversationId == conversation.Id).ToList();
            ViewBag.Conversations = db.Conversations.Where(m => (m.Participant1Id == userId || m.Participant2Id == userId) && (m.Hidden == false || m.Hidden == null)).ToList();
            ViewBag.Users = db.NhanViens.Where(u => u.MaNhanVien != userId).ToList();
            ViewBag.ConversationId = conversationId;
            return View(messages);
        }

        // POST: Message/SendMessage
        [HttpPost]
        public ActionResult SendMessage(int? conversationId, string content, HttpPostedFileBase image)
        {
            var userId = (string)Session["MaNhanVien"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "login"); // Redirect to login if session is null
            }

            if (conversationId == null)
            {
                // Fallback to global conversation ID if none is provided
                var globalConversation = db.Conversations.FirstOrDefault(c => c.IsGlobal == true);
                if (globalConversation != null)
                {
                    conversationId = globalConversation.Id;
                }
                else
                {
                    return HttpNotFound("Global conversation not found.");
                }
            }

            var conversation = db.Conversations.Find(conversationId);
            if (conversation == null)
            {
                return HttpNotFound("Conversation not found.");
            }

            // Determine the receiver ID based on the current user ID
            var receiverId = conversation.Participant1Id == userId ? conversation.Participant2Id : conversation.Participant1Id;

            if (string.IsNullOrEmpty(content) && image == null)
            {
                return RedirectToAction("Index", new { conversationId });
            }

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.Now,
                ConversationId = (int)conversationId
            };

            if (image != null)
            {
                var fileName = System.IO.Path.GetFileName(image.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/Content/images/messages"), fileName);
                image.SaveAs(path);
                message.ImagePath = "/Content/images/messages/" + fileName;
            }

            var sender = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == userId);
            var senderName = sender?.HoTen ?? "Admin";
            var senderAvatar = !string.IsNullOrEmpty(sender?.HinhAnh) ? "/Content/images/" + sender.HinhAnh : "/Content/images/default-avatar.png";
            var senderGender = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == userId).GioiTinh;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Chat>();
            hubContext.Clients.Group(conversationId.ToString()).receiveMessage(
                conversationId.ToString(),
                userId,
                senderName,
                senderAvatar,
                message.Content,
                message.ImagePath,
                message.SentAt,
                senderGender
            );



            db.Messages.Add(message);
            db.SaveChanges();
           
            return RedirectToAction("Index", new { conversationId });
        }


        // GET: Message/StartConversation
        public ActionResult StartConversation(string participantId)
        {
            ViewBag.otherUserNamePrivately = db.NhanViens.FirstOrDefault(m=>m.MaNhanVien == participantId).HoTen;
            var userId = (string)Session["MaNhanVien"];
            if (userId == null) 
            {
                return RedirectToAction("Login", "login");
            }

            ViewBag.Parti2 = participantId;
            var existingConversation = db.Conversations
                .FirstOrDefault(c =>
                    (c.Participant1Id == userId && c.Participant2Id == participantId) ||
                    (c.Participant1Id == participantId && c.Participant2Id == userId));

            if (existingConversation != null)
            {
                existingConversation.Hidden = false;
                db.SaveChanges();
                return RedirectToAction("Index", new { conversationId = existingConversation.Id });
            }

            var conversation = new Conversation
            {
                Participant1Id = userId,
                Participant2Id = participantId,
                IsGlobal = false,
                StartedAt = DateTime.Now
            };

            db.Conversations.Add(conversation);
            db.SaveChanges();

            return RedirectToAction("Index", new { conversationId = conversation.Id });
        }
        public ActionResult ListConversation(string receiverId)
        {
            var userId = (string)Session["MaNhanVien"];

            // Retrieve messages between the logged-in user and the selected receiver
            var messages = db.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.ReceiverId = receiverId;
            return View(messages);
        }
        public ActionResult ListUsers()
        {
            var userId = (string)Session["MaNhanVien"];
            var users = db.NhanViens.Where(u => u.MaNhanVien != userId).ToList(); // Exclude the current user
            return View(users);
        }
        [HttpPost]
        public ActionResult HideConversation(int conversationId)
        {
            var userId = (string)Session["MaNhanVien"];
            var conversation = db.Conversations.FirstOrDefault(c => c.Id == conversationId &&
                (c.Participant1Id == userId || c.Participant2Id == userId));

            if (conversation != null)
            {
                conversation.Hidden = true;
                db.SaveChanges();
            }

            return Json(new { success = true });
        }
    }
}
