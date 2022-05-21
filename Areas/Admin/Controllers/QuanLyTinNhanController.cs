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
    public class QuanLyTinNhanController : Controller
    {
        // GET: Admin/QuanLyTinNhan
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
        //Quản lý tin nhắn
        public ActionResult QuanLyTinNhan()
        {
            var t = db.TINNHAN_MAILs.ToList();
            return View(t);
        }

        public ActionResult DeleteTn(int ma)
        {
            foreach (var item in db.REPLY_TINNHAN_MAILs.Where(n => n.MATINNHAN == ma).ToList())
            {
                db.REPLY_TINNHAN_MAILs.DeleteOnSubmit(item);
                db.SubmitChanges();
            }
            var tn = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == ma);
            db.TINNHAN_MAILs.DeleteOnSubmit(tn);
            db.SubmitChanges();
            return RedirectToAction("QuanLyTinNhan", "QuanLyTinNhan");
        }
        public ActionResult DetailTn(int ma)
        {
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == ma);
            ViewBag.MATN = ma;
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.TIEUDE = b.TIEUDE;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.EMAIL = b.EMAIL;
            ViewBag.NGAYNHAN = b.NGAYNHAN;
            ViewBag.HINHANH = b.HINHANH;
            ViewBag.TINHTRANGPH = b.TINHTRANG_PHANHOI;
            return View();
        }
        public ActionResult PartialDetailListTnSended(int ma)
        {
            List<REPLY_TINNHAN_MAIL> listPh = new List<REPLY_TINNHAN_MAIL>();
            var s = db.REPLY_TINNHAN_MAILs.Where(n => n.MATINNHAN == ma).ToList();
            if (s != null)
            {
                ViewBag.STATE = "1";
                foreach (var item in s.ToList())
                {
                    listPh.Add(item);
                }
                return View(listPh.ToList());
            }
            else
            {
                ViewBag.STATE = "0";
                return View();
            }
        }

        [HttpGet]
        public ActionResult EditTn(int ma)
        {
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == ma);
            ViewBag.MATN = ma;
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.TIEUDE = b.TIEUDE;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.EMAIL = b.EMAIL;
            ViewBag.NGAYNHAN = b.NGAYNHAN;
            ViewBag.HINHANH = b.HINHANH;
            ViewBag.TINHTRANGPH = b.TINHTRANG_PHANHOI;
            return View();
        }
        [HttpPost]
        public ActionResult EditTn(FormCollection f)
        {
            if (f["HOTEN"].Equals(""))
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được để trống!";
                return View();
            } else if (f["HOTEN"].ToString().Length > 50)
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["TIEUDE"].Equals(""))
            {
                ViewBag.ErrorTIEUDE = "Tiêu đề không được để trống!";
                return View();
            } else if (f["TIEUDE"].ToString().Length > 100)
            {
                ViewBag.ErrorTIEUDE = "Tiêu đề không được vượt quá 100 kí tự!";
                return View();
            } else if (f["NOIDUNG"].Equals(""))
            {
                ViewBag.ErrorNOIDUNG = "Nội dung gửi đến khách hàng không được để trống!";
                return View();
            }
            else if (f["NOIDUNG"].ToString().Length > 2000)
            {
                ViewBag.ErrorNOIDUNG = "Nội dung không được vượt quá 2000 kí tự!";
                return View();
            }
            else if (f["EMAIL"].Equals(""))
            {
                ViewBag.ErrorEMAIL = "Vui lòng nhập email!";
                return View();
            }
            else if (!IsValidEmail(f["EMAIL"].ToString()))
            {
                ViewBag.ErrorEMAIL = "Vui lòng nhập đúng định dạng email!";
                return View();
            }
            else if (f["EMAIL"].ToString().Length > 50)
            {
                ViewBag.ErrorEMAIL = "Email không được vượt quá 50 kí tự!";
                return View();
            }
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == Convert.ToInt32(f["MATN"]));
            b.TIEUDE = f["TIEUDE"];
            b.NOIDUNG = f["NOIDUNG"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyTinNhan", "QuanLyTinNhan");
        }

        [HttpGet]
        public ActionResult GuiMailTn(int ma)
        {
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == ma);
            ViewBag.MATN = ma;
            ViewBag.EMAIL = b.EMAIL;
            ViewBag.HOTEN = b.HOTEN;
            return View();
        }
        [HttpPost]
        public ActionResult GuiMailTn(FormCollection f)
        {
            ViewBag.TIEUDE = f["TIEUDE"];
            ViewBag.NOIDUNG = f["NOIDUNG"];
            ViewBag.MATN = f["MATN"];
            ViewBag.EMAIL = f["EMAIL"];
            ViewBag.HOTEN = f["HOTEN"];
            if (f["TIEUDE"].Equals(""))
            {
                ViewBag.ErrorTIEUDE = "Tiêu đề không được để trống!";
                return View();
            }
            else if (f["NOIDUNG"].Equals(""))
            {
                ViewBag.ErrorNOIDUNG = "Nội dung gửi đến khách hàng không được để trống!";
                return View();
            }
            if (f["TIEUDE"].ToString().Length > 100)
            {
                ViewBag.ErrorTIEUDE = "Tiêu đề không được vượt quá 100 kí tự!";
                return View();
            }
            else if (f["NOIDUNG"].ToString().Length > 2000)
            {
                ViewBag.ErrorNOIDUNG = "Nội dung không được vượt quá 2000 kí tự!";
                return View();
            }
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == Convert.ToInt32(f["MATN"]));
            REPLY_TINNHAN_MAIL tn = new REPLY_TINNHAN_MAIL();
            tn.MATINNHAN = Convert.ToInt32(f["MATN"]);
            tn.TIEUDE = f["TIEUDE"];
            tn.NOIDUNG = f["NOIDUNG"];
            tn.NGAYTRALOI = DateTime.Now;
            db.REPLY_TINNHAN_MAILs.InsertOnSubmit(tn);
            b.TINHTRANG_PHANHOI = 1;
            db.SubmitChanges();


            var mail = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("m.contactqc@gmail.com", "loveyou3007@@"),
                EnableSsl = true
            };
            var message = new MailMessage();
            message.From = new MailAddress("m.contactqc@gmail.com");
            message.ReplyToList.Add("m.contactqc@gmail.com");
            message.To.Add(new MailAddress(f["EMAIL"]));
            message.Subject = f["TIEUDE"];
            message.Body = f["NOIDUNG"];
            mail.Send(message);
            return RedirectToAction("QuanLyTinNhan", "QuanLyTinNhan");
        }
    }
}