using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace QuanLyNhanSu.Areas.admin.Controllers
{
    public class HopDongController : Controller
    {
        // GET: admin/HopDong
        public QuanLyNhanSuEntities db = new QuanLyNhanSuEntities();
        public HopDongController()
        { 
            String session = System.Web.HttpContext.Current.Session["MaNhanVien"] as String;
            if (System.Web.HttpContext.Current.Session["MaNhanVien"] == null || session != "admin")
                {
                System.Web.HttpContext.Current.Response.Redirect("~/");
                }
        }
        // GET: HopDong
        public ActionResult Index()
        {
            var hopDongs = db.HopDongs.ToList();
            return View(hopDongs);
        }

        // GET: HopDong/Create
        public ActionResult Create()
        {
            var hopDong = new HopDong();
            var nhanViens = db.NhanViens.ToList();
            ViewBag.MaNhanVien = new SelectList(nhanViens, "MaNhanVien", "HoTen");
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( HopDong hopDong)
        {
            if (ModelState.IsValid)
            {
                NhanVien nv = db.NhanViens.SingleOrDefault(n => n.MaNhanVien == hopDong.MaNhanVien);
                nv.MaHopDong = hopDong.MaHopDong;

                db.HopDongs.Add(hopDong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            return View(hopDong);
        }

        private void PopulateNhanVienDropDownList(HopDong hopDong)
        {
            var nhanViens = db.NhanViens.ToList();
            ViewBag.MaNhanVien = new SelectList(nhanViens, "MaNhanVien", "HoTen");
        }

        // GET: HopDong/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HopDong hopDong = db.HopDongs.Find(id);
            if (hopDong == null)
            {
                return HttpNotFound();
            }
            var nhanViens = db.NhanViens.ToList();
            ViewBag.MaNhanVien = new SelectList(nhanViens, "MaNhanVien", "HoTen");
            var hoten= db.NhanViens.SingleOrDefault(n=>n.MaHopDong == id);
            ViewBag.hoten = hoten.HoTen;
            ViewBag.ma = hoten.MaNhanVien;
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HopDong hopDong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hopDong).State = System.Data.Entity.EntityState.Modified;
                NhanVien nv = db.NhanViens.SingleOrDefault(n => n.MaNhanVien == hopDong.MaNhanVien);
                nv.MaHopDong = hopDong.MaHopDong;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateNhanVienDropDownList(hopDong);
            return View(hopDong);
        }

        // GET: HopDong/Delete/5
        public ActionResult Delete(string id)
        {
            HopDong hopDong = db.HopDongs.Find(id);
            if (hopDong == null)
            {
                return HttpNotFound();
            }
            return View(hopDong);
        }

        // POST: HopDong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HopDong hopDong = db.HopDongs.Find(id);
            db.HopDongs.Remove(hopDong);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: HopDong/Extend/5
        public ActionResult Extend(string id)
        {
            HopDong hopDong = db.HopDongs.Find(id);
            if (hopDong == null)
            {
                return HttpNotFound();
            }
            return View(hopDong);
        }

        // POST: HopDong/Extend/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Extend(string id, DateTime newEndDate)
        {
            HopDong hopDong = db.HopDongs.Find(id);
            if (hopDong != null)
            {
                hopDong.NgayKetThuc = newEndDate;
                db.Entry(hopDong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult XuatFileExel()
        {

            var ds = db.NhanViens.Where(n => n.MaNhanVien != "admin" && n.MaHopDong != null).ToList();
            var phong = db.PhongBans.ToList();
            var gv = new GridView();
            //===================================================
            DataTable dt = new DataTable();
            //Add Datacolumn
            DataColumn workCol = dt.Columns.Add("Mã hợp đồng", typeof(String));

            dt.Columns.Add("Loại hợp đồng", typeof(String));
            dt.Columns.Add("Ngày bắt đầu", typeof(String));
            dt.Columns.Add("Ngày kết thúc", typeof(String));
            dt.Columns.Add("Ghi chú", typeof(String));
            dt.Columns.Add("Mã nhân viên", typeof(String));
            dt.Columns.Add("Tên nhân viên", typeof(String));

            //Add in the datarow


            foreach (var item in ds)
            {
                DataRow newRow = dt.NewRow();
                newRow["Mã hợp đồng"] = item.HopDong.MaHopDong;
                newRow["Loại hợp đồng"] = item.HopDong.LoaiHopDong;
                newRow["Ngày bắt đầu"] = item.HopDong.NgayBatDau;
                newRow["Ngày kết thúc"] = item.HopDong.NgayKetThuc;
                newRow["Ghi chú"] = item.HopDong.GhiChu;
                newRow["Mã nhân viên"] = item.MaNhanVien;
                newRow["Tên nhân viên"] = item.HoTen;


                dt.Rows.Add(newRow);
            }

            //====================================================
            gv.DataSource = dt;
            // gv.DataSource = ds;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment; filename=danh-sach-hop-dong.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Redirect("/admin/HopDong");
        }
    }
}