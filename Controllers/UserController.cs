using EstateSolution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EstateSolution.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        dbBatDongSanDataContext data = new dbBatDongSanDataContext();
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
        [HttpGet]
        public ActionResult LoginPage(string id, string tb)
        {
            if (Convert.ToInt32(Request.QueryString["tb"]) == 0)
            {
                ViewBag.ThongBao = "Đăng ký hoàn tất.\n Vui lòng đăng nhập bằng tài khoản mới vừa tạo.";
            }
            //string state2 = tb.ToString();
            //if (tb!= null && tb.Equals("0"))
            //{
            //    ViewBag.ThongBaoTC = "Đăng ký hoàn tất.\n Vui lòng đăng nhập bằng tài khoản mới vừa tạo.";
            //}
            //return RedirectToAction("LoginPage", "User", new { id = Request.QueryString["id"], tb = Request.QueryString["tb"] });
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(FormCollection f, string id)
        {
            ViewBag.TAIKHOAN = f["TAIKHOAN"];

            var sUserName = f["TAIKHOAN"];
            var sPw = f["MATKHAU"];
            THANHVIEN tvs = data.THANHVIENs.SingleOrDefault(n => n.TAIKHOAN == sUserName && n.MATKHAU == GetMD5(sPw) && n.STATUS_DELETE != 0);

            if (tvs == null)
            {
                string state = id.ToString();
                THANHVIEN tvs2 = data.THANHVIENs.SingleOrDefault(n => n.TAIKHOAN == sUserName && n.STATUS_DELETE != 0);
                
                if (f["TAIKHOAN"].Equals(""))
                {
                    ViewBag.ErrorTAIKHOAN = "Tài khoản không được để trống!";
                }
                else if (f["TAIKHOAN"].ToString().Length > 50)
                {
                    ViewBag.ErrorTAIKHOAN = "Tài khoản không được vượt quá 50 kí tự!";
                }
                else if (f["MATKHAU"].Equals(""))
                {
                    ViewBag.ErrorMATKHAU = "Mật khẩu không được để trống!";
                }
                else if (tvs2 == null)
                {
                    ViewBag.ErrorTAIKHOAN = "Tài khoản không tồn tại trên hệ thống!";
                }
                
                else if (tvs2.MATKHAU.Equals(f["MATKHAU"].ToString()) == false)
                {
                    ViewBag.ErrorMATKHAU = "Mật khẩu không chính xác!";
                }
                


                //ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                if (f["ID"].Length > 0 && f["IdBds"].Length > 0)
                {
                    ViewBag.ID = Convert.ToInt32(f["ID"]);
                    ViewBag.IdBds = Convert.ToInt32(f["IdBds"]);
                }
                else
                {
                    ViewBag.ID = Convert.ToInt32(f["ID2"]);
                    ViewBag.IdBds = Convert.ToInt32(f["IdBds2"]);
                }
                
                
                return View();
                //return Redirect("~/User/LoginPage?id=" + Convert.ToInt32(state) + "&&tb=" + f["IdBds"].ToString());
                //return RedirectToAction("LoginPage", "User", new { id = Convert.ToInt32(state), tb = f["IdBds"].ToString() });
            }
            else
            {
                //string state = id.ToString();
                //int idBds = Convert.ToInt32(state);
                string state;
                if (f["ID"].Length > 0 && f["IdBds"].Length > 0)
                {
                    state = f["ID"].ToString();                 
                }
                else
                {
                    state = f["ID2"].ToString();
                }
                int idBds = Convert.ToInt32(state);

                if (String.IsNullOrEmpty(sUserName))
                {
                    ViewData["Err1"] = "Vui lòng nhập tài khoản";
                }
                else if (String.IsNullOrEmpty(sPw))
                {
                    ViewData["Err2"] = "Vui lòng nhập mật khẩu";
                }
                else
                {
                    THANHVIEN tv = data.THANHVIENs.SingleOrDefault(n => n.TAIKHOAN == sUserName && n.MATKHAU == GetMD5(sPw) && n.STATUS_DELETE != 0);
                    if (tv != null)
                    {
                        Session["TAIKHOAN"] = tv;
                        if (state.Equals("1"))
                        {
                            return RedirectToAction("Index", "Home"); // edit
                        }
                        else if (state.Equals("2"))
                        {
                            return RedirectToAction("DatHang", "GioHang"); // edit
                        }
                        else if (state.Equals("5"))
                        {
                            return RedirectToAction("DatCocNgay", "GioHang"); // edit
                        }
                        else if (state.Equals("3"))
                        {
                            return RedirectToAction("Index", "Home"); // edit
                        }
                        else if (state.Equals("4"))
                        {
                            return RedirectToAction("DangTin", "Home"); // edit
                        }
                        else if (state.Equals("6"))
                        {
                            return RedirectToAction("Detail", "Home", new { idBds = idBds, state = 0 }); // edit
                        }


                    }

                }

            }

            return RedirectToAction("LoginPage", "User", new { id = Request.QueryString["id"], tb = Request.QueryString["tb"]});
        }
        public ActionResult Logout()
        {
            Session["TAIKHOAN"] = null;
            return RedirectToAction("Index", "Home");
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        public ActionResult LoginPartial()
        {
            
            ViewBag.TongSoLuong = TongSoLuong();
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, THANHVIEN tv)
        {
            var sHoTenTV = collection["HoTenTV"];
            var sUserName = collection["UserName"];
            var sPw = collection["password"];
            var sPwNL = collection["confirmPassword"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var sGioiTinh = collection["GioiTinh"];
            var lFacebook = collection["Facebook"];

            string tmpName = "";
            var hoVaTen = "";  // SAVE DATA
            for (int i = 0; i < sHoTenTV.Length; i++)
            {
                if (sHoTenTV[i] == 32)
                {
                    tmpName += "-";
                    hoVaTen += " ";
                }
                else if (sHoTenTV[i] == 45)
                {
                    hoVaTen += " ";
                    tmpName += "-";
                }
                else
                {
                    tmpName += sHoTenTV[i].ToString();
                    hoVaTen += sHoTenTV[i].ToString();
                }
            }
            string tmpDiaChi = "";
            var diaChi = ""; // SAVE DATA
            for (int i = 0; i < sDiaChi.Length; i++)
            {
                if (sDiaChi[i] == 32)
                {
                    tmpDiaChi += "-";
                    diaChi += " ";
                }
                else if (sDiaChi[i] == 45)
                {
                    diaChi += " ";
                    tmpDiaChi += "-";
                }
                else
                {
                    tmpDiaChi += sDiaChi[i].ToString();
                    diaChi += sDiaChi[i].ToString();
                }
            }
            ViewBag.HoTenTV = tmpName.ToString();
            ViewData["UserName"] = collection["UserName"];
            ViewData["Password"] = collection["password"];
            ViewData["ConfirmPassword"] = collection["confirmPassword"];
            ViewData["DiaChi"] = tmpDiaChi.ToString();
            ViewData["Email"] = collection["Email"];
            ViewData["DienThoai"] = collection["DienThoai"];
            ViewData["GioiTinh"] = collection["GioiTinh"];
            ViewData["Facebook"] = collection["Facebook"];

            //var dNgayDK = String.Format("{0:MM/dd/yyyy}", getDate());
            if (String.IsNullOrEmpty(sHoTenTV))
            {
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            
            else if (sHoTenTV.Length > 50)
            {
                ViewData["err1"] = "Họ tên không được vượt quá 50 kí tự!";
            }
            else if (!IsValidEmail(sEmail))
            {
                ViewData["err5"] = "Vui lòng nhập đúng định dạng email!";
                return View();
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }
            else if (data.THANHVIENs.SingleOrDefault(n => n.EMAIL == sEmail) != null)
            {
                ViewData["err5"] = "Email đã được sử dụng";
            }
            else if (sEmail.Length > 50)
            {
                ViewData["err5"] = "Email không được vượt quá 50 kí tự!";
            }
            else if (String.IsNullOrEmpty(sDiaChi)) 
            {
                ViewData["err6"] = "Địa chỉ không được rỗng";
            }
            else if (sDiaChi.Length > 50)
            {
                ViewData["err6"] = "Địa chỉ không được vượt quá 50 kí tự!";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err7"] = "Số điện thoại không được rỗng";
            }
            else if (sDienThoai.Length > 15)
            {
                ViewData["err7"] = "Số điện thoại không được vượt quá 15 kí tự!";
            }
            else if (sDienThoai.Length < 10)
            {
                ViewData["err7"] = "Số điện thoại phải tối thiểu 10 kí tự!";
            }
            else if (lFacebook.Length > 255)
            {
                ViewData["err15"] = "Link facebook không được vượt quá 255 kí tự!";
            }
            else if (String.IsNullOrEmpty(sUserName))
            {
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }
            else if (data.THANHVIENs.SingleOrDefault(n => n.TAIKHOAN == sUserName) != null)
            {
                ViewData["err10"] = "Tên đăng nhập đã tồn tại";
            }
            else if (sUserName.Length > 50)
            {
                ViewData["err2"] = "Tên đăng nhập không được vượt quá 50 kí tự!";
            }
            else if (String.IsNullOrEmpty(sPw))
            {
                ViewData["err3"] = "Mật khẩu không được để trống";
            }
            else if (sPw.Length < 6)
            {
                ViewData["err3"] = "Mật khẩu phải có tối thiểu là 6 kí tự!";
            }
            else if (sPw.Length > 50)
            {
                ViewData["err3"] = "Mật khẩu không được vượt quá 50 kí tự!";
            }
            else if (String.IsNullOrEmpty(sPwNL))
            {
                ViewData["err4"] = "Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(sGioiTinh))
            {
                ViewData["err8"] = "Vui lòng chọn giới tính";
            }
            else if (sPw.ToString().Equals(sPwNL.ToString()) == false)
            {
                ViewData["err9"] = "Mật khẩu không trùng khớp";
            }
            

            else
            {
                tv.TENTHANHVIEN = hoVaTen.ToString();
                tv.GIOITINH = sGioiTinh;
                tv.DIACHI = diaChi.ToString();
                tv.EMAIL = sEmail;
                tv.DIENTHOAI = sDienThoai;
                tv.NGAYDKTK = @DateTime.Now;
                tv.TAIKHOAN = sUserName;
                tv.MATKHAU = GetMD5(sPw);
                tv.LINK_FACEBOOK = lFacebook;
                tv.STATUS_DELETE = 1;
                data.THANHVIENs.InsertOnSubmit(tv);
                data.SubmitChanges();
                return Redirect("~/User/LoginPage?id=3&&tb=0");
            }
            return this.Register();
        }
    }
}