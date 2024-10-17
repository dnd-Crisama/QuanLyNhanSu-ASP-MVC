using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhanSu.Controllers
{
    public class ProfileController : Controller
    {
        private QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        // GET: UserProfile
        public ActionResult Index(string maNhanVien)
        {/*
            var user = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            var posts = db.Posts.Where(p => p.UserID == maNhanVien).OrderByDescending(p => p.PostDate).ToList();
            var photos = db.Photos.Where(p => p.UserID == maNhanVien).OrderByDescending(p => p.UploadDate).Take(6).ToList();

            var viewModel = new ProfileViewModel
            {
                User = user,
                Posts = posts,
                Photos = photos
            };*/

            var user = db.NhanViens
                .Include("Posts.Comments")
                .Include("Posts.Likes")
                .Include("Photos")
                .FirstOrDefault(u => u.MaNhanVien == maNhanVien);

            if (user == null)
                return HttpNotFound();

            return View(user);

        }

        // Post a new status

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(Post post, HttpPostedFileBase imageFile)
        {
            var id = (string)Session["MaNhanVien"];
            if (ModelState.IsValid)
            {
                post.PostDate = DateTime.Now;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/profile"), fileName);
                    imageFile.SaveAs(path);
                    post.ImageURL = "/Content/images/profile/" + fileName;
                }

                db.Posts.Add(post);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    postId = post.PostID,
                    userName = db.NhanViens.SingleOrDefault(m => m.MaNhanVien == id).HoTen,
                    userAvatar = db.NhanViens.SingleOrDefault(m => m.MaNhanVien == id).HinhAnh,
                    content = post.Content,
                    imageUrl = post.ImageURL 
                });
            }

            return RedirectToAction("index", new { maNhanVien = id });
        }

        // Like a post
        [HttpPost]
        public ActionResult ToggleLike(int postId)
        {
            var id = (string)Session["MaNhanVien"];
            var existingLike = db.Likes
                .FirstOrDefault(l => l.PostID == postId && l.UserID == id);

            if (existingLike != null)
            {
                db.Likes.Remove(existingLike);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    isLiked = existingLike,
                    likeCount = db.Likes.Where(m=>m.PostID == postId).Count()
                });
            }

            var like = new Like
            {
                PostID = postId,
                UserID = id,
                LikeDate = DateTime.Now
            };
                
            db.Likes.Add(like);
            db.SaveChanges();
            Post post = db.Posts.Find(postId);
            post.LikesCount = db.Likes.Where(l => l.PostID == postId).Count();
             
            db.SaveChanges();
            return Json(new
            {
                success = true,
                isLiked = existingLike,
                likeCount = post.LikesCount
            });
        }

        // Comment on a post
        [HttpPost]
        public ActionResult AddComment(int postId, string content)
        {
            var id = (string)Session["MaNhanVien"];
            var comment = new Comment
            {
                PostID = postId,
                UserID = id,
                Content = content,
                CommentDate = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            return Json(new
            {
                success = true,
                userName = db.NhanViens.SingleOrDefault(m=>m.MaNhanVien == id).HoTen,
                userAvatar = db.NhanViens.SingleOrDefault(m => m.MaNhanVien == id).HinhAnh,
                content = comment.Content
            });
        }
        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase photoFile)
        {
            var id = (string)Session["MaNhanVien"];

            if (photoFile != null && photoFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(photoFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/profile"), fileName);
                photoFile.SaveAs(path);

                var photo = new Photo
                {
                    UserID = id,
                    ImageURL = "/Content/images/profile/" + fileName,
                    UploadDate = DateTime.Now
                };
                
                db.Photos.Add(photo);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { maNhanVien = id });
        }
        [HttpPost]
        public JsonResult UpdateBackground(HttpPostedFileBase backgroundImage)
        {
            if (backgroundImage == null || backgroundImage.ContentLength == 0)
            {
                return Json(new { success = false, message = "No image provided" });
            }

            try
            {
                var userId = (string)Session["MaNhanVien"];
                var user = db.NhanViens.Find(userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Validate file type
                string[] allowedTypes = { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(backgroundImage.ContentType))
                {
                    return Json(new { success = false, message = "Invalid file type" });
                }

                // Generate unique filename
                string fileName = $"bg_{userId}_{DateTime.Now.Ticks}{Path.GetExtension(backgroundImage.FileName)}";
                string path = Path.Combine(Server.MapPath("~/Content/images/profile"), fileName);

                // Save file
                backgroundImage.SaveAs(path);

                // Update user's background image in database
                user.CoverImage = fileName;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    imageUrl = $"/Content/images/profile/{fileName}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating background image" });
            }
        }
    }
}

