using QuanLyNhanSu.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class AdminController : AuthorController
    {
        QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();

        //
        // GET: /admin/Admin/
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return Redirect("/");
        }
    }
}