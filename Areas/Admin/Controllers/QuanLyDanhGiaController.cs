using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EstateSolution.Models;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
//using GSF.Net.Smtp;
using Mail = EstateSolution.Models.Mail;
using Rotativa;
using Newtonsoft.Json;

namespace EstateSolution.Areas.Admin.Controllers
{
    public class QuanLyDanhGiaController : Controller
    {
        // GET: Admin/QuanLyDanhGia

        dbBatDongSanDataContext db = new dbBatDongSanDataContext();
        public int isEmptyNumber(int number, int condition)
        {
            if (number.Equals(""))
            {
                return 1;
            }
            else if (number <= condition)
            {
                return 2;
            }
            return 0;
        }
        public int isEmptyOrConditionNotMatch(string str, int condition)
        {
            try
            {
                if (str.Length == 0)
                {
                    return 1;
                }
                else if (str.Length > condition)
                {
                    return 2;
                }
            }
            catch (Exception e)
            {

            }
            return 0;
        }
        public ActionResult ListDonPartial(int month)
        {
            var ct = db.CHITIETDONDATCOCs.Where(n => n.DONDATCOC.NGAYHT.Value.Month == month && n.STATE_DELETE != 0).ToList();
            if (ct.Count() == 0)
            {
                ViewBag.SoLuongDon = 0;
                ViewBag.Tb = "Chưa có đơn đặt cọc nào thuộc loại này...";
            }
            ViewBag.THANG = month;
            return View(ct.ToList());
        }

        //Quản lý đánh giá bình luận của người dùng
        public ActionResult QuanLyDanhGia()
        {
            var b = db.BINHLUANDANHGIAs.Where(n => n.STATUS_DELETE != 0).ToList();
            return View(b);
        }
        public ActionResult DeleteDg(int ma)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma);
            var r = db.ADMIN_REPLies.SingleOrDefault(n => n.MABL == ma);
            if (r != null)
            {
                db.ADMIN_REPLies.DeleteOnSubmit(r);
                db.SubmitChanges();
            }
            db.BINHLUANDANHGIAs.DeleteOnSubmit(b);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "QuanLyDanhGia");
        }
        public ActionResult DetailDg(int ma, int maBds)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma);
            ViewBag.MABL = ma;
            ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == maBds).TENBDS;
            ViewBag.HINHANH = db.BDS.SingleOrDefault(n => n.MABDS == maBds).HINHANH;
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.NGAYBL = b.NGAYBL;
            ViewBag.SOSAO = b.SOSAO;
            var p = db.ADMIN_REPLies.Where(n => n.MABL == ma && n.MABDS == maBds).Count();
            if (p > 0)
            {
                ViewBag.TINHTRANGPH = "Đã phản hồi";
            }
            else
            {
                ViewBag.TINHTRANGPH = "Chưa phản hồi";
            }

            return View();
        }
        [HttpGet]
        public ActionResult EditDg(int ma)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma);
            ViewBag.MABL = ma;
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == ma).TENBDS;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.SOSAO = b.SOSAO;
            return View();
        }
        [HttpPost]
        public ActionResult EditDg(FormCollection f)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == Convert.ToInt32(f["MABL"]));
            b.NOIDUNG = f["NOIDUNG"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "QuanLyDanhGia");
        }
        public ActionResult PartialPhanHoi(int ma, int maBds)
        {
            ViewBag.MABL = ma;
            ViewBag.MABDS = maBds;
            var b = db.ADMIN_REPLies.SingleOrDefault(n => n.MABL == ma && n.MABDS == maBds);//
            if (b != null)
            {
                ViewBag.STATE = "1";
                ViewBag.NOIDUNGPH = b.NOIDUNG;
            }
            else
            {
                ViewBag.STATE = "0";
                ViewBag.NOIDUNGPH = "Chưa phản hồi";
            }
            return View();
        }
        [HttpGet]
        public ActionResult ReplyDg(int ma, int maBds, int maNv)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma && n.MABDS == maBds);
            if (b != null)
            {
                ViewBag.MABL = ma;
                ViewBag.MABDS = maBds;
                ViewBag.HOTEN = b.HOTEN;
                ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == b.MABDS).TENBDS;
                ViewBag.TENNV = db.NHANVIENs.SingleOrDefault(n => n.MANV == maNv).HOTEN;
                ViewBag.NOIDUNG = b.NOIDUNG;
                ViewBag.SOSAO = b.SOSAO;
                ViewBag.MANV = maNv;
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult ReplyDg(FormCollection f)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == Convert.ToInt32(f["MABL"].ToString()) && n.MABDS == Convert.ToInt32(f["MABDS"].ToString()));
            ViewBag.MABL = Convert.ToInt32(f["MABL"].ToString());
            ViewBag.MABDS = Convert.ToInt32(f["MABDS"].ToString());
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == b.MABDS).TENBDS;
            ViewBag.TENNV = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"].ToString())).HOTEN;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.SOSAO = b.SOSAO;
            ViewBag.MANV = Convert.ToInt32(f["MANV"].ToString());
            if (f["NOIDUNGPH"].Equals(""))
            {
                ViewBag.ErrorNOIDUNGPH = "Nội dug phản hồi không được để trống!";
                return View();
            }
            else if (f["NOIDUNGPH"].ToString().Length > 1000)
            {
                ViewBag.NOIDUNGPH = f["NOIDUNGPH"];
                ViewBag.ErrorNOIDUNGPH = "Nội dug phản hồi không được vượt quá 1000 kí tự!";
                return View();
            }
            ADMIN_REPLY a = new ADMIN_REPLY();
            a.MABL = Convert.ToInt32(f["MABL"]);
            a.MABDS = Convert.ToInt32(f["MABDS"]);
            a.MANV = Convert.ToInt32(f["MANV"]);
            a.NOIDUNG = f["NOIDUNGPH"];
            a.NGAYTRALOI = DateTime.Now;
            db.ADMIN_REPLies.InsertOnSubmit(a);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "QuanLyDanhGia");
        }
        [HttpGet]
        public ActionResult EditAdminReplyDg(int ma, int maBds, int maNv)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma && n.MABDS == maBds);
            if (b != null)
            {
                ViewBag.MABL = ma;
                ViewBag.MABDS = maBds;
                ViewBag.HOTEN = b.HOTEN;
                ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == b.MABDS).TENBDS;
                ViewBag.NOIDUNG = b.NOIDUNG;
                ViewBag.SOSAO = b.SOSAO;
                ViewBag.TENNV = db.NHANVIENs.SingleOrDefault(n => n.MANV == maNv).HOTEN;
                var p = db.ADMIN_REPLies.SingleOrDefault(n => n.MABL == ma && n.MABDS == maBds);
                ViewBag.NOIDUNGPH = p.NOIDUNG;
                ViewBag.MAREPLY = p.MAREPLY;
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditAdminReplyDg(FormCollection f)
        {
            if (f["NOIDUNGPH"].Equals(""))
            {
                ViewBag.ErrorNOIDUNGPH = "Nội dug phản hồi không được để trống!";
                return View();
            }
            else if (f["NOIDUNGPH"].ToString().Length > 1000)
            {
                ViewBag.NOIDUNGPH = f["NOIDUNGPH"];
                ViewBag.ErrorNOIDUNGPH = "Nội dug phản hồi không được vượt quá 1000 kí tự!";
                return View();
            }
            ADMIN_REPLY a = db.ADMIN_REPLies.SingleOrDefault(n => n.MAREPLY == Convert.ToInt32(f["MAREPLY"]));
            a.NOIDUNG = f["NOIDUNGPH"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "QuanLyDanhGia");
        }
    }
}