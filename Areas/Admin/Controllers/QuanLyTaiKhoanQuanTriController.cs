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
    public class QuanLyTaiKhoanQuanTriController : Controller
    {
        // GET: Admin/QuanLyTaiKhoanQuanTri
        dbBatDongSanDataContext db = new dbBatDongSanDataContext();
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
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

        //Quản lý tài khoản quản trị
        public ActionResult QuanLyTaiKhoanQuanTri()
        {
            var tk = db.NHANVIENs.ToList();
            return View(tk);
        }
        [HttpGet]
        public ActionResult CreateTk(int ma)
        {
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            ViewBag.MANV = ma;
            ViewBag.HOTEN = nv.HOTEN;
            return View();
        }
        public bool checkAccountExitted(string username)
        {
            NHANVIEN count;
            count = db.NHANVIENs.SingleOrDefault(n => n.TAIKHOAN == username);
            if (count != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public ActionResult CreateTk(FormCollection f)
        {
            ViewBag.TAIKHOAN = f["TAIKHOAN"].ToString();
            //ViewBag.MATKHAU = f["MATKHAU"].ToString();
            ViewBag.MANV = f["MANV"].ToString();
            ViewBag.HOTEN = f["HOTEN"].ToString();
            NHANVIEN nvTk = db.NHANVIENs.SingleOrDefault(n => n.TAIKHOAN == (f["TAIKHOAN"]).ToString());
            
            if (f["TAIKHOAN"].Equals(""))
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản không được để trống!";
                return View();
            }
            else if ((f["TAIKHOAN"].ToString()).Length > 50)
            {
                ViewData["err2"] = "Tên đăng nhập không được vượt quá 50 kí tự!";
            }
            else if (f["MATKHAU"].Equals(""))
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được để trống!";
                return View();
            }
            else if (f["MATKHAU"].ToString().Length < 6)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu phải có tối thiểu là 6 kí tự!";
                return View();
            }
            else if (f["MATKHAU"].ToString().Length > 50)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được vượt quá 50 kí tự!";
                return View();
            }
            else if (checkAccountExitted(f["TAIKHOAN"].ToString()) == true && f["TAIKHOAN"].ToString().Length > 0)
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản đã tồn tại!";
                ViewBag.TAIKHOAN = "";
                return View();
            }

            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"]));
            if (f["TAIKHOAN"].Equals("") && f["MATKHAU"].Equals(""))
            {
                nv.TAIKHOAN = null;
                nv.MATKHAU = null;
            }
            else
            {
                nv.TAIKHOAN = f["TAIKHOAN"];
                nv.MATKHAU = GetMD5(f["MATKHAU"]);
            }

            db.SubmitChanges();
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "QuanLyTaiKhoanQuanTri");
        }
        public ActionResult DeleteTk(int ma)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            nv.TAIKHOAN = null;
            nv.MATKHAU = null;
            db.SubmitChanges();
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "QuanLyTaiKhoanQuanTri");
        }
        public ActionResult DetailTk(int ma)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            ViewBag.MANV = nv.MANV;
            ViewBag.HOTEN = nv.HOTEN;
            ViewBag.TAIKHOAN = nv.TAIKHOAN;
            ViewBag.MATKHAU = nv.MATKHAU;
            ViewBag.CHUCVU = nv.CHUCVU;
            return View();
        }

        [HttpGet]
        public ActionResult EditTk(int ma)
        {
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            ViewBag.MANV = nv.MANV;
            ViewBag.HOTEN = nv.HOTEN;
            ViewBag.TAIKHOAN = nv.TAIKHOAN;
            ViewBag.MATKHAU = nv.MATKHAU;
            return View();
        }
        [HttpPost]
        public ActionResult EditTk(FormCollection f)
        {
            ViewBag.MANV = f["MANV"];
            ViewBag.HOTEN = f["HOTEN"];
            ViewBag.TAIKHOAN = f["TAIKHOAN"];

            if (f["MATKHAU"].ToString().Length < 6)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu phải có tối thiểu là 6 kí tự!";
                return View();
            }
            else if (f["MATKHAU"].Equals(""))
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được để trống!";
                return View();
            }
            else if (f["MATKHAU"].ToString().Length > 50)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được vượt quá 50 kí tự!";
                return View();
            }
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"]));
            if (nv.MATKHAU.Equals((f["MATKHAU"]).ToString()) == false && (f["MATKHAU"]).ToString().Equals("") == false)
            {
                nv.TAIKHOAN = f["TAIKHOAN"];
                nv.MATKHAU = GetMD5(f["MATKHAU"]);
                db.SubmitChanges();
            }
            else
            {
                nv.TAIKHOAN = f["TAIKHOAN"];
                db.SubmitChanges();
            }
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "QuanLyTaiKhoanQuanTri");
        }
    }
}