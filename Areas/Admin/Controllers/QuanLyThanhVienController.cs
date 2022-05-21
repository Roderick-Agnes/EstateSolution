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
    public class QuanLyThanhVienController : Controller
    {
        // GET: Admin/QuanLyThanhVien
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
        //Duyệt bds: DUYET != 0 && MATV > 0

        //Quản lý thành viên
        public ActionResult QuanLyThanhVien()
        {
            var tv = db.THANHVIENs.Where(n => n.STATUS_DELETE != 0).ToList();
            return View(tv);
        }
        [HttpGet]
        public ActionResult CreateTv()
        {
            string[] gt = { "Nam", "Nữ" };
            ViewBag.GIOITINH = new SelectList(gt.ToList());
            return View();
        }
        [HttpPost]
        public ActionResult CreateTv(FormCollection f, HttpPostedFileBase ANHDAIDIEN)
        {
            ViewBag.HOTEN = f["HOTEN"].ToString();
            ViewBag.DIACHI = f["DIACHI"].ToString();
            ViewBag.DIENTHOAI = f["DIENTHOAI"].ToString();
            ViewBag.EMAIL = f["EMAIL"].ToString();
            ViewBag.TAIKHOAN = f["TAIKHOAN"].ToString();
            //ViewBag.MATKHAU = f["MATKHAU"].ToString();
            ViewBag.FACEBOOK = f["FACEBOOK"].ToString();
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
            else if (f["HOTEN"].ToString().Length > 50)
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được vượt quá 50 kí tự!";
                return View();
            }
            
            else if (f["DIACHI"].Equals(""))
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được để trống!";
                return View();
            }
            else if (f["DIACHI"].ToString().Length > 50)
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được vượt quá 50 kí tự!";
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
            else if (f["EMAIL"].ToString().Length > 50)
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
            
            else if (f["TAIKHOAN"].Equals(""))
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản không được để trống!";
                return View();
            }
            else if (f["TAIKHOAN"].ToString().Length > 50)
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản không được vượt quá 50 kí tự!";
                return View();
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
            
            

            foreach (var item in db.THANHVIENs.ToList())
            {
                if (item.EMAIL.Equals(f["EMAIL"]))
                {
                    ViewBag.HOTEN = f["HOTEN"];
                    ViewBag.DIACHI = f["DIACHI"];
                    ViewBag.DIENTHOAI = f["DIENTHOAI"];
                    ViewBag.EMAIL = "";
                    ViewBag.TAIKHOAN = f["TAIKHOAN"];
                    //ViewBag.MATKHAU = f["MATKHAU"];
                    ViewBag.FACEBOOK = f["FACEBOOK"];
                    string[] gt = { "Nam", "Nữ" };
                    if (f["GIOITINH"].Equals("Nữ"))
                    {
                        ViewBag.GIOITINH = new SelectList(gt.ToList(), "Nữ");
                    }
                    else
                    {
                        ViewBag.GIOITINH = new SelectList(gt.ToList());
                    }
                    return RedirectToAction("CreateTv", "QuanLyThanhVien", new { tbe = "Email đã tồn tại" });
                }
                if (item.TAIKHOAN.Equals(f["TAIKHOAN"]))
                {
                    ViewBag.HOTEN = f["HOTEN"];
                    ViewBag.DIACHI = f["DIACHI"];
                    ViewBag.EMAIL = f["EMAIL"];
                    ViewBag.DIENTHOAI = f["DIENTHOAI"];
                    ViewBag.TAIKHOAN = "";
                    //ViewBag.MATKHAU = f["MATKHAU"];
                    ViewBag.FACEBOOK = f["FACEBOOK"];
                    string[] gt = { "Nam", "Nữ" };
                    if (f["GIOITINH"].Equals("Nữ"))
                    {
                        ViewBag.GIOITINH = new SelectList(gt.ToList(), "Nữ");
                    }
                    else
                    {
                        ViewBag.GIOITINH = new SelectList(gt.ToList());
                    }
                    return RedirectToAction("CreateTv", "QuanLyThanhVien", new { tbt = "Tài khoản đã tồn tại" });
                }


            }

            if (ANHDAIDIEN != null)
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(ANHDAIDIEN.FileName);
                    var path = Path.Combine(Server.MapPath("~/assets/Upload-Anh-Dai-Dien"), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        ANHDAIDIEN.SaveAs(path);
                    }
                    THANHVIEN tv = new THANHVIEN();
                    tv.TENTHANHVIEN = f["HOTEN"];
                    tv.GIOITINH = f["GIOITINH"];
                    tv.DIACHI = f["DIACHI"];
                    tv.EMAIL = f["EMAIL"];
                    tv.DIENTHOAI = f["DIENTHOAI"];
                    tv.NGAYDKTK = DateTime.Now;
                    tv.TAIKHOAN = f["TAIKHOAN"];
                    tv.MATKHAU = GetMD5(f["MATKHAU"]);
                    tv.LINK_FACEBOOK = f["FACEBOOK"];
                    tv.ANH_DAI_DIEN = fileName.ToString();
                    tv.STATUS_DELETE = 1;
                    db.THANHVIENs.InsertOnSubmit(tv);
                    db.SubmitChanges();
                    return RedirectToAction("QuanLyThanhVien", "QuanLyThanhVien");
                }
            }
            else
            {

                THANHVIEN tv = new THANHVIEN();
                tv.TENTHANHVIEN = f["HOTEN"];
                tv.GIOITINH = f["GIOITINH"];
                tv.DIACHI = f["DIACHI"];
                tv.EMAIL = f["EMAIL"];
                tv.DIENTHOAI = f["DIENTHOAI"];
                tv.NGAYDKTK = DateTime.Now;
                tv.TAIKHOAN = f["TAIKHOAN"];
                tv.MATKHAU = GetMD5(f["MATKHAU"]);
                tv.LINK_FACEBOOK = f["FACEBOOK"];
                tv.ANH_DAI_DIEN = "";
                tv.STATUS_DELETE = 1;
                db.THANHVIENs.InsertOnSubmit(tv);
                db.SubmitChanges();
                return RedirectToAction("QuanLyThanhVien", "QuanLyThanhVien");
            }
            return RedirectToAction("QuanLyThanhVien", "QuanLyThanhVien");
        }
        public ActionResult DeleteTv(int ma)
        {
            var bds = db.BDS.Where(n => n.MATV == ma).ToList();
            int x = 1;
            foreach (var item in bds)
            {
                x--;
                item.MATV = x;
                db.SubmitChanges();
            }

            var don = db.CHITIETDONDATCOCs.Where(n => n.MATV == ma).ToList();
            if (don != null)
            {
                foreach (var item in don)
                {
                    db.CHITIETDONDATCOCs.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }


            var bl = db.BINHLUANDANHGIAs.Where(n => n.MATV == ma).ToList();
            //int z = 1;
            foreach (var item in bl)
            {
                //z--;
                //item.MATV = z;
                item.STATUS_DELETE = 0;
                db.SubmitChanges();
            }

            var s = db.THANHVIENs.SingleOrDefault(n => n.MATV == ma);
            s.STATUS_DELETE = 0;
            //db.THANHVIENs.DeleteOnSubmit(s);
            db.SubmitChanges();
            return RedirectToAction("QuanLyThanhVien", "QuanLyThanhVien");
        }


        public ActionResult DetailTv(int ma)
        {
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == ma);
            ViewBag.MATV = ma;
            ViewBag.HOTEN = tv.TENTHANHVIEN;
            ViewBag.GIOITINH = tv.GIOITINH;
            ViewBag.DIACHI = tv.DIACHI;
            ViewBag.EMAIL = tv.EMAIL;
            ViewBag.DIENTHOAI = tv.DIENTHOAI;
            ViewBag.NGAYDKTK = tv.NGAYDKTK;
            ViewBag.TAIKHOAN = tv.TAIKHOAN;
            ViewBag.MATKHAU = tv.MATKHAU;
            ViewBag.LINK_FACEBOOK = tv.LINK_FACEBOOK;
            ViewBag.ANH_DAI_DIEN = tv.ANH_DAI_DIEN;
            return View();
        }
        [HttpGet]
        public ActionResult EditTv(int ma)
        {
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == ma);
            ViewBag.MATV = ma;
            ViewBag.HOTEN = tv.TENTHANHVIEN;
            string[] gt = { "Nam", "Nữ" };
            ViewBag.GIOITINH = new SelectList(gt.ToList(), tv.GIOITINH);
            ViewBag.DIACHI = tv.DIACHI;
            ViewBag.EMAIL = tv.EMAIL;
            ViewBag.DIENTHOAI = tv.DIENTHOAI;
            ViewBag.NGAYDKTK = tv.NGAYDKTK; //redonly
            ViewBag.TAIKHOAN = tv.TAIKHOAN;
            ViewBag.MATKHAU = tv.MATKHAU;
            ViewBag.LINK_FACEBOOK = tv.LINK_FACEBOOK;
            ViewBag.ANH_DAI_DIEN = tv.ANH_DAI_DIEN;
            return View();
        }
        [HttpPost]
        public ActionResult EditTv(FormCollection f, HttpPostedFileBase ANHDAIDIEN)
        {
            string[] gt = { "Nam", "Nữ" };
            ViewBag.GIOITINH = new SelectList(gt.ToList(), f["GIOITINH"]);
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
            else if (f["TAIKHOAN"].Equals(""))
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản không được để trống!";
                return View();
            }
            

            if (f["HOTEN"].ToString().Length > 50)
            {
                ViewBag.ErrorHOTEN = "Họ và tên không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["DIACHI"].ToString().Length > 50)
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["EMAIL"].ToString().Length > 50)
            {
                ViewBag.ErrorEMAIL = "Email không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["TAIKHOAN"].ToString().Length > 50)
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["MATKHAU"].ToString().Length > 50)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được vượt quá 50 kí tự!";
                return View();
            }
            else if (f["MATKHAU"].ToString().Length < 6)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu phải có tối thiểu là 6 kí tự!";
                return View();
            }
            else if (f["FACEBOOK"].ToString().Length > 255)
            {
                ViewBag.ErrorFACEBOOK = "Link facebook không được vượt quá 255 kí tự!";
                return View();
            }
            var accountExitted = db.THANHVIENs.SingleOrDefault(n => n.MATV != Convert.ToInt32(f["MATV"]) && n.TAIKHOAN.Equals(f["TAIKHOAN"]));
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == Convert.ToInt32(f["MATV"]));
            if (accountExitted != null)
            {
                ViewBag.ErrorTAIKHOAN = "Tài khoản đã tồn tại!";
                return View();
            }
            else
            {
                tv.TENTHANHVIEN = f["HOTEN"];
                tv.GIOITINH = f["GIOITINH"];
                tv.DIACHI = f["DIACHI"];
                tv.EMAIL = f["EMAIL"];
                tv.DIENTHOAI = f["DIENTHOAI"];
                tv.TAIKHOAN = f["TAIKHOAN"];
                if (tv.MATKHAU.Equals((f["MATKHAU"]).ToString()) == false && (f["MATKHAU"]).ToString().Equals("") == false)
                {
                    tv.MATKHAU = GetMD5(f["MATKHAU"]);
                    db.SubmitChanges();
                }
                tv.LINK_FACEBOOK = f["FACEBOOK"];
                //tv.ANH_DAI_DIEN = f[""];
            }

            if (ANHDAIDIEN != null)
            {
                var fileName = Path.GetFileName(ANHDAIDIEN.FileName);
                var path = Path.Combine(Server.MapPath("~/assets/Upload-Anh-Dai-Dien"), fileName);
                if (!System.IO.File.Exists(path))
                {
                    ANHDAIDIEN.SaveAs(path);
                }
                tv.ANH_DAI_DIEN = fileName.ToString();
                db.SubmitChanges();
            }
            db.SubmitChanges();
            return RedirectToAction("QuanLyThanhVien", "QuanLyThanhVien");
        }
    }
}