using QuanLyNhanSu.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyNhanSu.Controllers
{
    public class loginController : Controller
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        //
        // GET: /login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.err = "";
            return View();
        }
        [HttpPost]
        public ActionResult Login(NhanVien user)
        {
            var userFromDb = db.NhanViens.FirstOrDefault(x => x.MaNhanVien == user.MaNhanVien);
            //check email da ton tai chua
            var checkaccount = db.NhanViens.Any(x => x.MaNhanVien == user.MaNhanVien &&
                x.MatKhau == user.MatKhau && x.TrangThai == true);
            var checkadmin = db.NhanViens.Any(x => x.MaNhanVien == user.MaNhanVien &&
                    x.MaNhanVien == "admin" &&
                    x.MatKhau == user.MatKhau
                );

            if (checkaccount)
            {
                ViewBag.err = "";

                Session["MaNhanVien"] = user.MaNhanVien;
                Session["TenNV"] = userFromDb.HoTen;
                Session["HinhAnh"] = userFromDb.HinhAnh;

                

                FormsAuthentication.SetAuthCookie(user.MaNhanVien, false);
                if (checkadmin)
                {
                    return Redirect("~/admin/Admin/Index");
                }
                else
                {
                    return Redirect("/");
                }
            }

            else
            {
                Session["user"] = null;
                ViewBag.err = "Tài khoản hoặc mật khẩu không đúng";
                //  ModelState.AddModelError("Account", "Tài khoản hoặc mật khẩu không đúng...!");
                return View();
            }

        }

        [HttpGet]
        public ActionResult UpDateUser()
        {
            UserValidate up = new UserValidate();

            var id = Session["MaNhanVien"] as String;
            var us = db.NhanViens.Where(n => n.MaNhanVien == id).FirstOrDefault();
            if (us != null)
            {
                up.MaNhanVien = us.MaNhanVien;
                up.HinhAnh = us.HinhAnh;
                up.MatKhau = us.MatKhau;
                up.XacNhanMatKhau = us.MatKhau;
                up.HoTen = us.HoTen;
                up.NgaySinh = us.NgaySinh;
                up.QueQuan = us.QueQuan;
                up.GioiTinh = us.GioiTinh;
                up.DanToc = us.DanToc;
                up.sdt_NhanVien = us.sdt_NhanVien;
                up.MaChuyenNganh = us.MaChuyenNganh;
                up.Email = us.Email;
                up.Facebookk = us.Facebookk;
                up.Bio = us.Bio;

                up.CMND = us.CMND;


                return View(up);
            }
            return Redirect("~/");
        }
        [HttpPost]
        public ActionResult UpDateUser(UserValidate us, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                var up = db.NhanViens.Where(n => n.MaNhanVien == us.MaNhanVien).FirstOrDefault();
                up.MaNhanVien = us.MaNhanVien;

                up.MatKhau = us.MatKhau;
                up.MatKhau = us.XacNhanMatKhau;
                up.HoTen = us.HoTen;
                up.NgaySinh = us.NgaySinh;
                up.QueQuan = us.QueQuan;
                up.GioiTinh = us.GioiTinh;
                up.DanToc = us.DanToc;
                up.sdt_NhanVien = us.sdt_NhanVien;
                up.MaChuyenNganh = us.MaChuyenNganh;

                up.CMND = us.CMND;
                up.Email = us.Email;
                up.Facebookk = us.Facebookk;
                up.Bio = us.Bio;

                DateTime dateOfBirth;
                if (!DateTime.TryParseExact(us.NgaySinh.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
                {
                    ViewBag.ErrorMessage = "Ngày sinh không hợp lệ. Vui lòng nhập đúng định dạng MM-DD-YYYY.";
                }

                if (us.HinhAnh != null)
                {
                    HinhAnh.SaveAs(HttpContext.Server.MapPath("~/Content/images/")
                                                             + HinhAnh.FileName);
                    up.HinhAnh = HinhAnh.FileName;
                    us.HinhAnh = HinhAnh.FileName;
                    //user.Image = userVal.Image;
                }
                else
                {
                    us.HinhAnh = up.HinhAnh;
                }

                db.SaveChanges();
                return View(us);
            }
            else
            {
                return View(us);
            }
        }
        public ActionResult DangXuat()
        {
            //Đăng xuất khỏi ứng dụng
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            //Về trang chủ
            return Redirect("/");
        }
        public ActionResult FooterPartial()
        {
            return PartialView("_FooterPartial");
        }
        [HttpGet]
        public ActionResult ForgotPassword(string id)
        {
            var us = db.NhanViens.FirstOrDefault(m => m.MaNhanVien == id);
            if (us == null)
            {
                ViewBag.Error = "Mã nhân viên không tồn tại.";
            }
            return View(us);
        }

        [HttpPost]
        public ActionResult ForgotPassword(string maNhanVien, string email)
        {
            var user = db.NhanViens.FirstOrDefault(x => x.Email == email && x.MaNhanVien == maNhanVien);
            if (user != null)
            {
                // Generate a random verification code
                string verificationCode = Guid.NewGuid().ToString().Substring(0, 6);
                Session["VerificationCode"] = verificationCode;
                Session["ResetUserId"] = user.MaNhanVien;

                // Send the code to user's email
                // Assuming a method SendEmail exists that sends an email.
                SendVerificationCode(user.Email, verificationCode);

                ViewBag.Message = "Mã xác nhận đã được gửi tới email của bạn.";
                return RedirectToAction("VerifyCode");
            }
            else
            {
                ViewBag.Error = "Email hoặc mã nhân viên không chính xác.";
            }
            return View(db.NhanViens.FirstOrDefault(m=>m.MaNhanVien == maNhanVien));
        }

        [HttpGet]
        public ActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyCode(string code)
        {
            string sessionCode = Session["VerificationCode"] as string;
            if (sessionCode == code)
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "Mã xác nhận không chính xác.";
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword == confirmPassword)
            {
                string userId = Session["ResetUserId"] as string;
                var user = db.NhanViens.FirstOrDefault(x => x.MaNhanVien == userId);
                if (user != null)
                {
                    user.MatKhau = newPassword;
                    db.SaveChanges();

                    Session["VerificationCode"] = null;
                    Session["ResetUserId"] = null;

                    ViewBag.Message = "Mật khẩu của bạn đã được đặt lại thành công.";
                    return RedirectToAction("Login");
                }
            }

            ViewBag.Error = "Mật khẩu không khớp.";
            return View();
        }

        // Helper method to send email (you need to implement it)
        public void SendVerificationCode(string toEmail, string verificationCode)
        {
            try
            {
                var fromAddress = new MailAddress("tapphongloanvu@gmail.com", "Quản lý nhân sự");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "qtxt rkar buwx llid";
                string subject = "Password Reset Verification Code";
                string body = $"Your verification code is: {verificationCode}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
    
}
