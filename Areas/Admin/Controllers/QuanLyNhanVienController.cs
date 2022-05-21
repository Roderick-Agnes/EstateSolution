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
    public class QuanLyNhanVienController : Controller
    {
        // GET: Admin/QuanLyNhanVien
        dbBatDongSanDataContext db = new dbBatDongSanDataContext();
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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

        public ActionResult QuanLyNhanVien()
        {
            var s = db.NHANVIENs.OrderByDescending(n => n.MANV).ToList();
            return View(s);
        }

        [HttpGet]
        public ActionResult CreateNv()
        {
            string[] gt = { "Nam", "Nữ" };
            ViewBag.GIOITINH = new SelectList(gt.ToList());

            var pq = db.PHANQUYENs.ToList();
            ViewBag.MAPQ = new SelectList(pq.ToList(), "MAPQ", "CHUCVU");

            return View();
        }
        [HttpPost]
        public ActionResult CreateNv(FormCollection f)
        {
            ViewBag.HOTEN = f["HOTEN"];
            ViewBag.DIACHI = f["DIACHI"];
            ViewBag.EMAIL = f["EMAIL"].ToString();
            ViewBag.DIENTHOAI = f["DIENTHOAI"].ToString();
            ViewBag.FACEBOOK = f["FACEBOOK"].ToString();
            var pq = db.PHANQUYENs.ToList();
            ViewBag.MAPQ = new SelectList(pq.ToList(), "MAPQ", "CHUCVU");
            string gioiTinh = "Male";

            string[] gtt = { "Nam", "Nữ" };
            if (f["GIOITINH"].Equals("Nữ"))
            {
                ViewBag.GIOITINH = new SelectList(gtt.ToList(), "Nữ");
            }
            else
            {
                ViewBag.GIOITINH = new SelectList(gtt.ToList());
            }

            if (f["HOTEN"].Equals(""))
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được để trống!";
                return View();
            }
            else if (f["DIACHI"].Equals(""))
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được để trống!";
                return View();
            }
            else if (f["EMAIL"].Equals(""))
            {
                ViewBag.ErrorEMAIL = "Email không được để trống!";
                return View();
            }
            else if (!IsValidEmail(f["EMAIL"].ToString()))
            {
                ViewBag.ErrorEMAIL = "Vui lòng nhập đúng định dạng email!";
                return View();
            }
            else if ((f["EMAIL"].ToString()).Length > 50)
            {
                ViewBag.ErrorEMAIL = "Email không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["DIENTHOAI"].Equals(""))
            {
                ViewBag.ErrorDIENTHOAI = "Điện thoại không được để trống!";
                return View();
            }
            else if (f["DIENTHOAI"].ToString().Length > 15)
            {
                ViewBag.ErrorDIENTHOAI = "Số điện thoại không được vượt quá 15 kí tự!";
                return View();
            }
            else if (f["DIENTHOAI"].ToString().Length < 10)
            {
                ViewBag.ErrorDIENTHOAI = "Số điện thoại phải tối thiểu 10 kí tự!";
                return View();
            }
            NHANVIEN nv = new NHANVIEN();
            nv.HOTEN = f["HOTEN"];


            if (f["GIOITINH"].Equals("Nam"))
            {
                gioiTinh = "Male";
            }
            else
            {
                gioiTinh = "Female";
            }
            nv.GIOITINH = gioiTinh;
            nv.DIACHI = f["DIACHI"];
            nv.EMAIL = f["EMAIL"];
            nv.DIENTHOAI = f["DIENTHOAI"];
            nv.LINK_FACEBOOK = f["FACEBOOK"];
            nv.MAPQ = Convert.ToInt32(f["MAPQ"]);
            nv.CHUCVU = db.PHANQUYENs.SingleOrDefault(n => n.MAPQ == Convert.ToInt32(f["MAPQ"])).CHUCVU;

            db.NHANVIENs.InsertOnSubmit(nv);
            db.SubmitChanges();
            return RedirectToAction("QuanLyNhanVien", "QuanLyNhanVien");
        }
        public ActionResult DetailNv(int ma)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            ViewBag.MANV = ma;
            ViewBag.HOTEN = nv.HOTEN;
            ViewBag.GIOITINH = nv.GIOITINH;
            ViewBag.DIACHI = nv.DIACHI;
            ViewBag.EMAIL = nv.EMAIL;
            ViewBag.DIENTHOAI = nv.DIENTHOAI;
            if (nv.LINK_FACEBOOK.Equals(""))
            {
                ViewBag.FACEBOOK = "Chưa cập nhật";
            }
            else
            {
                ViewBag.FACEBOOK = nv.LINK_FACEBOOK;
            }
            ViewBag.CHUCVU = nv.CHUCVU;
            if (nv.TAIKHOAN == null && nv.MATKHAU == null)
            {
                ViewBag.TAIKHOAN = "Chưa cập nhật";
                ViewBag.MATKHAU = "Chưa cập nhật";
            }
            else
            {
                ViewBag.TAIKHOAN = nv.TAIKHOAN;
                ViewBag.MATKHAU = nv.MATKHAU;
            }

            return View();
        }
        public ActionResult DeleteNv(int ma)
        {
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            db.NHANVIENs.DeleteOnSubmit(nv);
            db.SubmitChanges();
            return RedirectToAction("QuanLyNhanVien", "QuanLyNhanVien");
        }

        [HttpGet]
        public ActionResult EditNv(int ma)
        {
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            ViewBag.MANV = ma;
            ViewBag.HOTEN = nv.HOTEN;
            ViewBag.DIACHI = nv.DIACHI;
            ViewBag.EMAIL = nv.EMAIL;
            ViewBag.DIENTHOAI = nv.DIENTHOAI;
            ViewBag.FACEBOOK = nv.LINK_FACEBOOK;
            ViewBag.CHUCVU = nv.CHUCVU;
            string[] gt = { "Nam", "Nữ" };
            ViewBag.GIOITINH = new SelectList(gt.ToList(), nv.GIOITINH);

            var pq = db.PHANQUYENs.ToList();
            ViewBag.MAPQ = new SelectList(pq.ToList(), "MAPQ", "CHUCVU", nv.MAPQ);
            return View();
        }
        [HttpPost]
        public ActionResult EditNv(FormCollection f)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"]));
            nv.HOTEN = f["HOTEN"];
            if (f["GIOITINH"].Equals("Nam"))
            {
                nv.GIOITINH = "Male";
            }
            else
            {
                nv.GIOITINH = "Female";
            }
            if (f["HOTEN"].Equals(""))
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được để trống!";
                return View();
            }
            else if (f["DIACHI"].Equals(""))
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được để trống!";
                return View();
            }
            else if (f["EMAIL"].Equals(""))
            {
                ViewBag.ErrorEMAIL = "Email không được để trống!";
                return View();
            }
            else if (!IsValidEmail(f["EMAIL"].ToString()))
            {
                ViewBag.ErrorEMAIL = "Vui lòng nhập đúng định dạng email!";
                return View();
            }
            else if ((f["EMAIL"].ToString()).Length > 50)
            {
                ViewBag.ErrorEMAIL = "Email không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["DIENTHOAI"].Equals(""))
            {
                ViewBag.ErrorDIENTHOAI = "Điện thoại không được để trống!";
                return View();
            }
            else if (f["DIENTHOAI"].ToString().Length > 15)
            {
                ViewBag.ErrorDIENTHOAI = "Số điện thoại không được vượt quá 15 kí tự!";
                return View();
            }
            else if (f["DIENTHOAI"].ToString().Length < 10)
            {
                ViewBag.ErrorDIENTHOAI = "Số điện thoại phải tối thiểu 10 kí tự!";
                return View();
            }
            nv.DIACHI = f["DIACHI"];
            nv.EMAIL = f["EMAIL"];
            nv.DIENTHOAI = f["DIENTHOAI"];
            nv.LINK_FACEBOOK = f["FACEBOOK"];
            nv.MAPQ = Convert.ToInt32(f["MAPQ"]);
            nv.CHUCVU = db.PHANQUYENs.SingleOrDefault(n => n.MAPQ == Convert.ToInt32(f["MAPQ"])).CHUCVU;

            db.SubmitChanges();
            return RedirectToAction("QuanLyNhanVien", "QuanLyNhanVien");
        }
    }
}