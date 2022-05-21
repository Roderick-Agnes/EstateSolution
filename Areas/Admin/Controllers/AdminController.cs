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
    public class AdminController : Controller
    {
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
            if(ct.Count() == 0)
            {
                ViewBag.SoLuongDon = 0;
                ViewBag.Tb = "Chưa có đơn đặt cọc nào thuộc loại này...";
            }
            ViewBag.THANG = month;
            return View(ct.ToList());
        }
        public ActionResult Menu()
        {
            return View();
        }
        // GET: Area/Home
        [HttpPost]
        public ActionResult GetMonthSelected(FormCollection f)
        {
            int tmpMonth = DateTime.Now.Month;
            string[] months = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
            List<DataPoint> dataPoints = new List<DataPoint>();
            if (f["months"].Length == 7)
            {
                tmpMonth = Convert.ToInt32(f["months"][6].ToString());
            }
            else
            {
                tmpMonth = Convert.ToInt32(f["months"][6].ToString() + f["months"][7].ToString());
            }
            var ct = db.DONDATCOCs.Where(n => n.NGAYHT.Value.Month == tmpMonth);
            foreach (var item in ct)
            {
                string tenLoai = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == item.MADON && n.STATE_DELETE != 0).LOAIBDS.TENLOAI;
                var loaiBds = db.LOAIBDS.ToList();
                foreach (var items in loaiBds)
                {
                    double doanThuTheoLoai = Convert.ToDouble(db.CHITIETDONDATCOCs.Where(n => n.MADON == item.MADON && n.MALOAI == items.MALOAI).Sum(n => n.TONGTIEN));
                    dataPoints.Add(new DataPoint(tenLoai, doanThuTheoLoai));
                }
                break;
            }

            ViewBag.months = new SelectList(months.ToList(), f["months"]);
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return RedirectToAction("Index", "Admin", new { month = Convert.ToInt32(tmpMonth)});
        }
        public ActionResult Index()
        {
            if (Session["NHANVIEN"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                //int tmpMonth = DateTime.Now.Month;
                //string[] months = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
                //List<DataPoint> dataPoints = new List<DataPoint>();
                //var ct = db.DONDATCOCs.Where(n => n.NGAYHT.Value.Month == tmpMonth);
                //foreach (var item in ct)
                //{
                //    string tenLoai = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == item.MADON).LOAIBDS.TENLOAI;
                //    var loaiBds = db.LOAIBDS.ToList();
                //    foreach (var items in loaiBds)
                //    {
                //        double doanhThuTheoLoai = Convert.ToDouble(db.CHITIETDONDATCOCs.Where(n => n.MADON == item.MADON && n.MALOAI == items.MALOAI).Sum(n => n.TONGTIEN));
                //        if(doanhThuTheoLoai <= 0)
                //        {
                //            doanhThuTheoLoai = 0;
                //        }
                //        dataPoints.Add(new DataPoint(tenLoai, doanhThuTheoLoai));
                //    }
                //    break;
                //}

                //ViewBag.months = new SelectList(months.ToList(), "Tháng " + tmpMonth.ToString());

                int month = Convert.ToInt32(Request.QueryString["month"]);
                //int month = Convert.ToInt32(DateTime.Now.Month);
                List<DataPoint> dataPoints = new List<DataPoint>();
                List<LOAIBDS> listLoai = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
                ViewBag.ListLoai = db.LOAIBDS.ToList();
                foreach(var item in listLoai)
                {
                    string ii = item.TENLOAI;
                    List <CHITIETDONDATCOC> listDDC = db.CHITIETDONDATCOCs.Where(n => n.DONDATCOC.NGAYHT.Value.Month == month && n.MALOAI == item.MALOAI).ToList();
                    if(listDDC == null)
                    {
                        dataPoints.Add(new DataPoint(item.TENLOAI, 0));
                    }
                    else
                    {
                        double tt = 0;
                        for (int i = 0; i < listDDC.Count(); i++)
                        {
                            //var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == listDDC[i].MADON && n.MALOAI == item.MALOAI);
                            tt += (double)listDDC[i].TONGTIEN;
                            //tt += (double)ct.TONGTIEN;
                        }
                        dataPoints.Add(new DataPoint(item.TENLOAI, tt*1000000000 + tt * 1000000000*10/100));
                    }
                    
                }



                ViewBag.MONTH = month;
                string[] months = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
                ViewBag.months = new SelectList(months.ToList(), "Tháng " + month.ToString());
                //dataPoints.Add(new DataPoint("Mercury", 36));
                //dataPoints.Add(new DataPoint("Venus", 67.2));
                //dataPoints.Add(new DataPoint("Earth", 93));
                //dataPoints.Add(new DataPoint("Mars", 141.6));
                //dataPoints.Add(new DataPoint("Jupiter", 483.6));
                //dataPoints.Add(new DataPoint("Saturn", 886.7));
                //dataPoints.Add(new DataPoint("Uranus", 1784));
                //dataPoints.Add(new DataPoint("Neptune", 2794.4));
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                return View();
            }
        }
        public ActionResult PartialCart(int month)
        {
            //if (month == null)
            //{
            //    string str = DateTime.Now.ToString().Trim();
            //    str = str.Substring(3, 2);
            //    month = Convert.ToInt32(str);
            //}
            ViewData["CountLoai"] = db.LOAIBDS.ToList().Count();
            var l = db.LOAIBDS.ToList();
            List<CartLoai> listData = new List<CartLoai>();
            List<LOAIBDS> listLoai = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            foreach (var item in listLoai)
            {
                string ii = item.TENLOAI;
                List<CHITIETDONDATCOC> listDDC = db.CHITIETDONDATCOCs.Where(n => n.DONDATCOC.NGAYHT.Value.Month == month && n.MALOAI == item.MALOAI).ToList();
                if (listDDC == null)
                {
                    listData.Add(new CartLoai(item.TENLOAI, 0, 0));
                }
                else
                {
                    double tt = 0;
                    for (int i = 0; i < listDDC.Count(); i++)
                    {
                        //var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == listDDC[i].MADON && n.MALOAI == item.MALOAI);
                        tt += (double)listDDC[i].TONGTIEN;
                        //tt += (double)ct.TONGTIEN;
                    }
                    listData.Add(new CartLoai(item.TENLOAI, listDDC.Count(), tt * 1000000000 + tt * 1000000000 * 10 / 100));
                }

            }
            return View(listData.ToList());
        }
        // Chức năng đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            ViewBag.TAIKHOAN = f["TAIKHOAN"];
            var userName = f["TAIKHOAN"];
            var pw = f["MATKHAU"];
            if (userName.Equals(""))
            {
                ViewBag.ThongBao = "Tên đăng nhập không được để trống!";
                return View();
            }
            else if (pw.Equals("")) {
                ViewBag.ThongBao = "Mật khẩu không được để trống!";
                return View();
            }
            NHANVIEN qt = db.NHANVIENs.SingleOrDefault(n => n.TAIKHOAN == userName && n.MATKHAU == GetMD5(pw));
            if (qt != null)
            {
                Session["NHANVIEN"] = qt;
                if (qt.TAIKHOAN.Equals(f["TAIKHOAN"]) == true && qt.MATKHAU.Equals(GetMD5(pw)) == true)  // edit
                {
                    
                    return RedirectToAction("Index", "Admin", new { month = Convert.ToInt32(DateTime.Now.Month)});
                }
                else if (!qt.TAIKHOAN.Equals(f["TAIKHOAN"]) || !qt.MATKHAU.Equals(GetMD5(pw)))
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    return View();
                }
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập không tồn tại!";
                return View();
            }
            return View();
        }
        public ActionResult AreaInfo()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["NHANVIEN"] = null;
            return RedirectToAction("Login", "Admin");
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

        //Đổi mật khẩu
        [HttpGet]
        public ActionResult DoiMatKhau(int id)
        {
            var tp = db.NHANVIENs.SingleOrDefault(n => n.MANV == id);//id
            if (tp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DoiMatKhau(int id, FormCollection f)
        {
            if (f["Pwm"].ToString().Length < 6)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu phải có tối thiểu là 6 kí tự!";
                return View();
            }
            else if (f["Pwm"].ToString().Length > 50)
            {
                ViewBag.ErrorMATKHAU = "Mật khẩu không được vượt quá 50 kí tự!";
                return View();
            }
            String tmpPwc, tmpPw1, tmpPw2;
            var tp = db.NHANVIENs.SingleOrDefault(n => n.MANV == id);//id
            if (ModelState.IsValid)
            {
                tmpPwc = f["Pw"];
                tmpPw1 = f["Pwm"];
                tmpPw2 = f["xnpw"];
                if (tmpPw1.Equals(tmpPw2) == true && tp.MATKHAU.Equals(GetMD5(tmpPwc)) == true)
                {
                    tp.MATKHAU = GetMD5(f["xnpw"]);
                    db.SubmitChanges();
                    return RedirectToAction("DoiMatKhauTC", "Admin");
                }
                else if (tp.MATKHAU.Equals(GetMD5(tmpPwc)) == false)
                {
                    ViewBag.ThongBao1 = "Mật khẩu cũ không đúng!";
                    return View();
                }
                else if (tmpPw1.Equals(tmpPw2) == false)
                {
                    ViewBag.ThongBao2 = "Mật khẩu mới không trùng khớp!";
                    return View();
                }


            }
            return View(tp);
        }
        //Thông báo đổi mật khẩu thành công
        public ActionResult DoiMatKhauTC()
        {
            return View();
        }
    }
}