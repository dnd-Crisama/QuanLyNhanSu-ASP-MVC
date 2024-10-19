using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Controllers
{
    public class NewsfeedController : Controller
    {
        private QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        public ActionResult Index()
        {
            var currentUserId = Session["MaNhanVien"] as string;
            var posts = db.Posts
                .OrderByDescending(p => p.PostDate)
                .Take(20)
                .ToList();

            var users = db.NhanViens.Where(nv => nv.MaNhanVien != currentUserId).ToList();
            var employeeNames = users.ToDictionary(u => u.MaNhanVien, u => u.HoTen);
            var employeeId = users.ToDictionary(u => u.MaNhanVien, u => u.MaNhanVien);

            // SORT USER BY MESSAGE
            var usersWithMessageCount = db.NhanViens
                .Where(u => u.MaNhanVien != currentUserId) // Exclude the current user
                .Select(u => new
                {
                 User = u,
                MessageCount = db.Messages.Count(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == u.MaNhanVien) ||
                    (m.SenderId == u.MaNhanVien && m.ReceiverId == currentUserId)
                )
                })
                .OrderByDescending(x => x.MessageCount) // Sort by message count
                .ToList();


            // Pass the sorted users and data to the view
            ViewBag.UsersMess = usersWithMessageCount.Select(x => x.User).ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Users = users;
            ViewBag.EmployeeNames = employeeNames;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(currentUserId);

            return View(posts);
        }

        [HttpPost]
        public ActionResult ToggleLike(int postId)
        {
            var currentUserId = Session["MaNhanVien"] as string;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "login") });
            }

            var existingLike = db.Likes.FirstOrDefault(l => l.PostID == postId && l.UserID == currentUserId);

            if (existingLike == null)
            {
                db.Likes.Add(new Like { PostID = postId, UserID = currentUserId });
            }
            else
            {
                db.Likes.Remove(existingLike);
            }

            db.SaveChanges();

            var likeCount = db.Likes.Count(l => l.PostID == postId);
            return Json(new { success = true, likeCount = likeCount ,isLiked = existingLike});
        }

        [HttpPost]
        public ActionResult AddComment(int postId, string content)
        {
            var currentUserId = Session["MaNhanVien"] as string;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "login") });
            }

            var comment = new Comment
            {
                PostID = postId,
                UserID = currentUserId,
                Content = content,
                CommentDate = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            var userName = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == currentUserId)?.HoTen;
            var userAvatar = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == currentUserId)?.HinhAnh ?? "icon.jpg";

            return Json(new
            {
                success = true,
                commentDate = comment.CommentDate.ToString(),
                userName = userName,
                userAvatar = userAvatar
            });
        }
        [HttpPost]
        public ActionResult PostStatus(string content, HttpPostedFileBase image)
        {
            var currentUserId = Session["MaNhanVien"] as string;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "login") });
            }

            var post = new Post
            {
                UserID = currentUserId,
                Content = content,
                PostDate = DateTime.Now
            };

            if (image != null && image.ContentLength > 0)
            {
                var fileName = Path.GetFileName(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/profile"), fileName);
                image.SaveAs(path);
                post.ImageURL = "/Content/images/profile/" + fileName;
            }

            db.Posts.Add(post);
            db.SaveChanges();

            var userName = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == currentUserId)?.HoTen;
            var userAvatar = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == currentUserId)?.HinhAnh ?? "icon.jpg";

            return Json(new
            {
                success = true,
                postId = post.PostID,
                userName = userName,
                userAvatar = userAvatar,
                postDate = post.PostDate.ToString(),
                content = post.Content,
                imageUrl = post.ImageURL,
                userId = currentUserId
            });
        }
    }
}