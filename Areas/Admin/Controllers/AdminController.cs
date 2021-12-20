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
            var userName = f["TAIKHOAN"];
            var pw = f["MATKHAU"];
            NHANVIEN qt = db.NHANVIENs.SingleOrDefault(n => n.TAIKHOAN == userName && n.MATKHAU == (pw));
            if (qt != null)
            {
                Session["NHANVIEN"] = qt;
                if (qt.TAIKHOAN.Equals(f["TAIKHOAN"]) == true && qt.MATKHAU.Equals(f["MATKHAU"]) == true)
                {
                    
                    return RedirectToAction("Index", "Admin", new { month = Convert.ToInt32(DateTime.Now.Month)});
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    return View();
                }

            }
            ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
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
            String tmpPwc, tmpPw1, tmpPw2;
            var tp = db.NHANVIENs.SingleOrDefault(n => n.MANV == id);//id
            if (ModelState.IsValid)
            {
                tmpPwc = f["Pw"];
                tmpPw1 = f["Pwm"];
                tmpPw2 = f["xnpw"];
                if (tmpPw1.Equals(tmpPw2) == true && tp.MATKHAU.Equals(tmpPwc) == true)
                {
                    tp.MATKHAU = f["xnpw"];
                    db.SubmitChanges();
                    return RedirectToAction("DoiMatKhauTC", "Admin");
                }
                else if (tp.MATKHAU.Equals(tmpPwc) == false)
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







        //Quản lý bất động sản
        public ActionResult QuanLyBDS()
        {
            var s = db.BDS.OrderByDescending(n => n.MABDS).Where(m => m.LOAIBDS.STATUS_DELETE == 1).ToList();
            return View(s);
        }
        public ActionResult DuyetBds(int ma, int state, int manv)
        {
            var b = db.BDS.SingleOrDefault(n => n.MABDS == ma);
            if(state != 1)
            {
                b.STATE_DELETE = 1;
                b.MA_AD_DUYET = manv;
                b.DUYET = 1;
                db.SubmitChanges();
            }
            else
            {
                b.STATE_DELETE = 1;
                b.MA_AD_DUYET = 1;
                b.DUYET = 0;
                db.SubmitChanges();
            }
            return RedirectToAction("QuanLyBds", "Admin");
        }

        [HttpGet]
        public ActionResult EditBds(int ma)
        {
            string[] tr = { "Đã duyệt", "Bỏ duyệt" };
            var l = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(ma));
            ViewBag.MALOAI = new SelectList(db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList(), "MALOAI", "TENLOAI", l.MALOAI_BDS);
            //ViewBag.manv = id.ToString();
            ViewBag.MABDS = ma;
            ViewBag.TENBDS = l.TENBDS;
            ViewBag.THONGTIN = l.THONGTIN;
            ViewBag.GIA = l.GIA;
            ViewBag.COC_TRUOC = l.COC_TRUOC;
            ViewBag.NHANVIENDUYET = l.NHANVIEN.HOTEN;// Convert.ToInt32(id);
            ViewBag.NGAYDANG = l.NGAYDANG;
            //ViewBag.TINHTRANGDUYET = new SelectList(tr.ToList());
            ViewBag.NGUOIDANG = l.THANHVIEN.TENTHANHVIEN;

            DACDIEM_BDS d = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == ma);
            ViewBag.DIACHI = d.DIACHI;
            ViewBag.DIENTICH = d.DIENTICH;
            ViewBag.SOPHONGNGU = d.SOPHONGNGU;
            ViewBag.SOPHONGTAM = d.SOPHONGTAM;
            ViewBag.SOGARA = d.SOGARA;
            ViewBag.PHAPLY = d.PHAPLY;

            ViewBag.SoLuongDanhGia = db.BINHLUANDANHGIAs.Count(n => n.MABDS == ma);
            var sao = (from bl in db.BINHLUANDANHGIAs where bl.MABDS == ma select bl).Sum(n => n.SOSAO);
            if (sao == null)
            {
                ViewBag.TongDanhGia = 0;
            }
            else
            {
                ViewBag.TongDanhGia = (from bl in db.BINHLUANDANHGIAs where bl.MABDS == ma select bl).Sum(n => n.SOSAO);
            }

            ViewBag.SoLuongAnh = db.HINH_ANH_BDS.Where(n => n.MABDS == ma).Count();


            return View();
        }
        [HttpPost]
        public ActionResult EditBds(FormCollection f, HttpPostedFileBase fileUploadNew, HttpPostedFileBase fileUploadNew2, HttpPostedFileBase fileUploadNew3)
        {
            if (fileUploadNew != null || fileUploadNew2 != null || fileUploadNew3 != null)
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUploadNew.FileName);
                    var fileName2 = Path.GetFileName(fileUploadNew2.FileName);
                    var fileName3 = Path.GetFileName(fileUploadNew3.FileName);
                    var path = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName);
                    var path2 = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName2);
                    var path3 = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName3);
                    if (!System.IO.File.Exists(path))
                    {
                        fileUploadNew.SaveAs(path);
                    }
                    if (!System.IO.File.Exists(path2))
                    {
                        fileUploadNew2.SaveAs(path2);
                    }
                    if (!System.IO.File.Exists(path3))
                    {
                        fileUploadNew3.SaveAs(path3);
                    }
                    BDS mabds = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
                    mabds.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                    mabds.TENBDS = f["TENBDS"];
                    mabds.THONGTIN = f["THONGTIN"];
                    mabds.GIA = Convert.ToDecimal(f["GIA"]);
                    mabds.COC_TRUOC = Convert.ToInt32(f["COCTRUOC"]);
                    //mabds.MATV = 2; //Edit người đăng tin
                    mabds.HINHANH = fileName.ToString();
                    db.SubmitChanges();

                    DACDIEM_BDS dd = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
                    dd.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                    dd.DIACHI = f["DIACHI"];
                    dd.DIENTICH = f["DIENTICH"];
                    dd.SOPHONGNGU = Convert.ToInt32(f["SOPHONGNGU"]);
                    dd.SOPHONGTAM = Convert.ToInt32(f["SOPHONGTAM"]);
                    dd.SOGARA = Convert.ToInt32(f["SOGARA"]);
                    dd.PHAPLY = f["PHAPLY"];
                    db.SubmitChanges();

                    var itemHa = db.HINH_ANH_BDS.Where(n => n.MABDS == Convert.ToInt32(f["MABDS"])).ToList();
                    int i = -1;
                    String[] newListHa = { fileName, fileName2, fileName3 };
                    foreach (var item in itemHa)
                    {
                        i++;
                        if(i < newListHa.Length)
                        {
                            item.HINHANH = newListHa[i].ToString();// fileName.ToString();
                            db.SubmitChanges();
                        }
                        
                    }
                    db.SubmitChanges();
                    
                    return RedirectToAction("QuanLyBDS", "Admin");
                }
            }
            BDS mabdss = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
            mabdss.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
            mabdss.TENBDS = f["TENBDS"];
            mabdss.THONGTIN = f["THONGTIN"];
            mabdss.GIA = Convert.ToDecimal(f["GIA"]);
            mabdss.COC_TRUOC = Convert.ToInt32(f["COCTRUOC"]);
            //mabds.MATV = 2; //Edit người đăng tin
            //mabds.HINHANH = fileName.ToString();
            db.SubmitChanges();

            DACDIEM_BDS d = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
            d.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
            d.DIACHI = f["DIACHI"];
            d.DIENTICH = f["DIENTICH"];
            d.SOPHONGNGU = Convert.ToInt32(f["SOPHONGNGU"]);
            d.SOPHONGTAM = Convert.ToInt32(f["SOPHONGTAM"]);
            d.SOGARA = Convert.ToInt32(f["SOGARA"]);
            d.PHAPLY = f["PHAPLY"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyBDS", "Admin");
        }

        public ActionResult DetailBds(int ma)
        {
            BDS b = db.BDS.SingleOrDefault(n => n.MABDS == ma);
            ViewBag.MABDS = ma;
            ViewBag.TENLOAI = b.LOAIBDS.TENLOAI;
            ViewBag.TENBDS = b.TENBDS;
            ViewBag.THONGTIN = b.THONGTIN;
            ViewBag.GIA = b.GIA;
            ViewBag.COC_TRUOC = b.COC_TRUOC;
            ViewBag.NHANVIENDUYET = b.NHANVIEN.HOTEN;// Convert.ToInt32(id);
            ViewBag.NGAYDANG = b.NGAYDANG;
            //if(b.DUYET != 0)
            //{
            //    ViewBag.TINHTRANGDUYET = "Đã duyệt";
            //}
            //else
            //{
            //    ViewBag.TINHTRANGDUYET = "Chưa duyệt";
            //}
            //ViewBag.SAO = b.SAO;
            ViewBag.NGUOIDANG = b.THANHVIEN.TENTHANHVIEN;

            DACDIEM_BDS d = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == ma);
            ViewBag.DIACHI = d.DIACHI;
            ViewBag.DIENTICH = d.DIENTICH;
            ViewBag.SOPHONGNGU = d.SOPHONGNGU;
            ViewBag.SOPHONGTAM = d.SOPHONGTAM;
            ViewBag.SOGARA = d.SOGARA;
            ViewBag.PHAPLY = d.PHAPLY;

            ViewBag.SoLuongDanhGia = db.BINHLUANDANHGIAs.Count(n => n.MABDS == ma);
            var sao = (from bl in db.BINHLUANDANHGIAs where bl.MABDS == ma select bl).Sum(n => n.SOSAO);
            if (sao == null)
            {
                ViewBag.TongDanhGia = 0;
            }
            else
            {
                ViewBag.TongDanhGia = (from bl in db.BINHLUANDANHGIAs where bl.MABDS == ma select bl).Sum(n => n.SOSAO);
            }

            ViewBag.SoLuongAnh = db.HINH_ANH_BDS.Where(n => n.MABDS == ma).Count();

            return View();
        }
        public ActionResult DetailBdsImage(int ma)
        {
            var s = db.HINH_ANH_BDS.Where(n => n.MABDS == Convert.ToInt32(ma)).ToList();
            return View(s);
        }
        [HttpGet]
        public ActionResult CreateBds()
        {
            var list = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
            //ViewBag.manv = id.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult CreateBds(FormCollection f, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (fileUpload == null && fileUpload2 == null && fileUpload3 == null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return RedirectToAction("CreateBds", "Admin", new { ma = Convert.ToInt32(f["maad"])});
            }
            if(fileUpload == null && fileUpload2 != null && fileUpload3 != null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return RedirectToAction("CreateBds", "Admin", new { ma = Convert.ToInt32(f["maad"]) });
            }
            if (fileUpload != null && fileUpload2 == null && fileUpload3 != null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return RedirectToAction("CreateBds", "Admin", new { ma = Convert.ToInt32(f["maad"]) });
            }
            if (fileUpload != null && fileUpload2 != null && fileUpload3 == null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return RedirectToAction("CreateBds", "Admin", new { ma = Convert.ToInt32(f["maad"]) });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var fileName2 = Path.GetFileName(fileUpload2.FileName);
                    var fileName3 = Path.GetFileName(fileUpload3.FileName);
                    var path = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName);
                    var path2 = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName2);
                    var path3 = Path.Combine(Server.MapPath("~/assets/img/img-data"), fileName3);
                    if (!System.IO.File.Exists(path))
                    {
                        fileUpload.SaveAs(path);
                    }
                    if (!System.IO.File.Exists(path2))
                    {
                        fileUpload2.SaveAs(path2);
                    }
                    if (!System.IO.File.Exists(path3))
                    {
                        fileUpload3.SaveAs(path3);
                    }
                    var mabds = db.BDS.OrderByDescending(n => n.MABDS).First();
                    BDS b = new BDS();
                    b.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                    b.TENBDS = f["TENBDS"];
                    b.THONGTIN = f["THONGTIN"];
                    b.GIA = Convert.ToDecimal(f["GIA"]);
                    b.COC_TRUOC = Convert.ToInt32(f["COCTRUOC"]);
                    b.MA_AD_DUYET = 1;//Convert.ToInt32(f["maad"]);// Convert.ToInt32(id);
                    b.NGAYDANG = DateTime.Now;
                    b.DUYET = 0;
                    b.STATE_DELETE = 1;
                    b.SAO = 0;
                    b.MATV = 2; //Edit người đăng tin//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    b.HINHANH = fileName.ToString();
                    db.BDS.InsertOnSubmit(b);
                    db.SubmitChanges();

                    DACDIEM_BDS d = new DACDIEM_BDS();
                    d.MABDS = mabds.MABDS + 1;
                    d.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                    d.DIACHI = f["DIACHI"];
                    d.DIENTICH = f["DIENTICH"];
                    d.SOPHONGNGU = Convert.ToInt32(f["SOPHONGNGU"]);
                    d.SOPHONGTAM = Convert.ToInt32(f["SOPHONGTAM"]);
                    d.SOGARA = Convert.ToInt32(f["SOGARA"]);
                    d.PHAPLY = f["PHAPLY"];
                    db.DACDIEM_BDS.InsertOnSubmit(d);
                    db.SubmitChanges();

                    HINH_ANH_BDS h1 = new HINH_ANH_BDS();
                    h1.MABDS = mabds.MABDS + 1;
                    h1.HINHANH = fileName.ToString();
                    db.HINH_ANH_BDS.InsertOnSubmit(h1);
                    db.SubmitChanges();

                    HINH_ANH_BDS h2 = new HINH_ANH_BDS();
                    h2.MABDS = mabds.MABDS + 1;
                    h2.HINHANH = fileName2.ToString();
                    db.HINH_ANH_BDS.InsertOnSubmit(h2);
                    db.SubmitChanges();

                    HINH_ANH_BDS h3 = new HINH_ANH_BDS();
                    h3.MABDS = mabds.MABDS + 1;
                    h3.HINHANH = fileName3.ToString();
                    db.HINH_ANH_BDS.InsertOnSubmit(h3);
                    db.SubmitChanges();

                    return RedirectToAction("QuanLyBDS", "Admin");
                }
                return View();
            }
        }

        public ActionResult DeleteBds(int ma)
        {
            List<BINHLUANDANHGIA> bl = db.BINHLUANDANHGIAs.Where(n => n.MABDS == ma).ToList();
            List<ADMIN_REPLY> adr = db.ADMIN_REPLies.Where(n => n.MABDS == ma).ToList();
            if(adr != null)
            {
                foreach(var item in adr)
                {
                    db.ADMIN_REPLies.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
                
            }
            if(bl != null)
            {
                foreach (var item in bl)
                {
                    db.BINHLUANDANHGIAs.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }
            List<CHITIETDONDATCOC> ct = db.CHITIETDONDATCOCs.Where(n => n.MABDS == ma).ToList();
            if(ct != null)
            {
                foreach (var item in ct)
                {
                    db.CHITIETDONDATCOCs.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            //DACDIEM_BDS d = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == ma);
            //db.DACDIEM_BDS.DeleteOnSubmit(d);
            //db.SubmitChanges();

            List<HINH_ANH_BDS> ha = db.HINH_ANH_BDS.Where(n => n.MABDS == ma).ToList();
            if (ha != null)
            {
                foreach (var item in ha)
                {
                    db.HINH_ANH_BDS.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }
            var nv = db.BDS.SingleOrDefault(n => n.MABDS == ma);
            nv.STATE_DELETE = 0;
            nv.DUYET = 0;
            //db.BDS.DeleteOnSubmit(nv);
            db.SubmitChanges();

            return RedirectToAction("QuanLyBds", "Admin");
        }



        //Quản lý loại bất động sản
        public ActionResult QuanLyLoaiBDS()
        {
            var s = db.LOAIBDS.OrderByDescending(n => n.MALOAI).Where(m => m.STATUS_DELETE == 1).ToList();
            return View(s);
        }
        [HttpGet]
        public ActionResult CreateLoaiBds()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateLoaiBds(FormCollection f)
        {
            LOAIBDS l = new LOAIBDS();
            l.TENLOAI = f["TENLOAI"];
            l.STATUS_DELETE = 1;
            db.LOAIBDS.InsertOnSubmit(l);
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "Admin");
        }
        public ActionResult DeleteLoaiBds(int ma)
        {
            var s = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == ma);
            //db.LOAIBDS.DeleteOnSubmit(s);
            s.STATUS_DELETE = 0;
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "Admin");
        }
        public ActionResult DetailLoaiBds(int ma)
        {
            var s = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == ma);
            ViewBag.MALOAI = s.MALOAI;
            ViewBag.TENLOAI = s.TENLOAI;
            return View();
        }
        [HttpGet]
        public ActionResult EditLoaiBds(int ma)
        {
            var s = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == ma);
            ViewBag.MALOAI = s.MALOAI;
            ViewBag.TENLOAI = s.TENLOAI;
            return View();
        }
        [HttpPost]
        public ActionResult EditLoaiBds(FormCollection f)
        {
            LOAIBDS l = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == Convert.ToInt32(f["MALOAI"]));
            l.TENLOAI = f["TENLOAI"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "Admin");
        }



        //Quản lý nhân viên
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
            NHANVIEN nv = new NHANVIEN();
            nv.HOTEN = f["HOTEN"];
            if (f["GIOITINH"].Equals("Nam"))
            {
                nv.GIOITINH = "Male";
            }
            else
            {
                nv.GIOITINH = "Female";
            }
            nv.DIACHI = f["DIACHI"];
            nv.EMAIL = f["EMAIL"];
            nv.DIENTHOAI = f["DIENTHOAI"];
            nv.LINK_FACEBOOK = f["FACEBOOK"];
            nv.MAPQ = Convert.ToInt32(f["MAPQ"]);
            nv.CHUCVU = db.PHANQUYENs.SingleOrDefault(n => n.MAPQ == Convert.ToInt32(f["MAPQ"])).CHUCVU;

            db.NHANVIENs.InsertOnSubmit(nv);
            db.SubmitChanges();
            return RedirectToAction("QuanLyNhanVien", "Admin");
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
            if(nv.TAIKHOAN == null && nv.MATKHAU == null)
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
            return RedirectToAction("QuanLyNhanVien", "Admin");
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
            nv.DIACHI = f["DIACHI"];
            nv.EMAIL = f["EMAIL"];
            nv.DIENTHOAI = f["DIENTHOAI"];
            nv.LINK_FACEBOOK = f["FACEBOOK"];
            nv.MAPQ = Convert.ToInt32(f["MAPQ"]);
            nv.CHUCVU = db.PHANQUYENs.SingleOrDefault(n => n.MAPQ == Convert.ToInt32(f["MAPQ"])).CHUCVU;

            db.SubmitChanges();
            return RedirectToAction("QuanLyNhanVien", "Admin");
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
        [HttpPost]
        public ActionResult CreateTk(FormCollection f)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"]));
            if (f["TAIKHOAN"].Equals("") && f["MATKHAU"].Equals(""))
            {
                nv.TAIKHOAN = null;
                nv.MATKHAU = null;
            }
            else
            {
                nv.TAIKHOAN = f["TAIKHOAN"];
                nv.MATKHAU = f["MATKHAU"];
            }
            
            db.SubmitChanges();
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "Admin");
        }
        public ActionResult DeleteTk(int ma)
        {
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == ma);
            nv.TAIKHOAN = null;
            nv.MATKHAU = null;
            db.SubmitChanges();
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "Admin");
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
            var nv = db.NHANVIENs.SingleOrDefault(n => n.MANV == Convert.ToInt32(f["MANV"]));
            nv.TAIKHOAN = f["TAIKHOAN"];
            nv.MATKHAU = f["MATKHAU"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyTaiKhoanQuanTri", "Admin");
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
            foreach (var item in db.THANHVIENs.ToList())
            {
                if (item.EMAIL.Equals(f["EMAIL"]))
                {
                    ViewBag.HOTEN = f["HOTEN"];
                    ViewBag.DIACHI = f["DIACHI"];
                    ViewBag.DIENTHOAI = f["DIENTHOAI"];
                    ViewBag.EMAIL = "";
                    ViewBag.TAIKHOAN = f["TAIKHOAN"];
                    ViewBag.MATKHAU = f["MATKHAU"];
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
                    return RedirectToAction("CreateTv", "Admin", new { tbe = "Email đã tồn tại", ht = f["HOTEN"], dc = f["DIACHI"], dt = f["DIENTHOAI"], e = "", tk = f["TAIKHOAN"], mk = f["MATKHAU"], fb = f["FACEBOOK"]});
                }
                if (item.TAIKHOAN.Equals(f["TAIKHOAN"]))
                {
                    ViewBag.HOTEN = f["HOTEN"];
                    ViewBag.DIACHI = f["DIACHI"];
                    ViewBag.EMAIL = f["EMAIL"];
                    ViewBag.DIENTHOAI = f["DIENTHOAI"];
                    ViewBag.TAIKHOAN = "";
                    ViewBag.MATKHAU = f["MATKHAU"];
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
                    return RedirectToAction("CreateTv", "Admin", new { tbt = "Tài khoản đã tồn tại", ht = f["HOTEN"], dc = f["DIACHI"], dt = f["DIENTHOAI"], e = f["EMAIL"], tk = "", mk = f["MATKHAU"], fb = f["FACEBOOK"] });
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
                    tv.MATKHAU = f["MATKHAU"];
                    tv.LINK_FACEBOOK = f["FACEBOOK"];
                    tv.ANH_DAI_DIEN = fileName.ToString();
                    tv.STATUS_DELETE = 1;
                    db.THANHVIENs.InsertOnSubmit(tv);
                    db.SubmitChanges();
                    return RedirectToAction("QuanLyThanhVien", "Admin");
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
                    tv.MATKHAU = f["MATKHAU"];
                    tv.LINK_FACEBOOK = f["FACEBOOK"];
                    tv.ANH_DAI_DIEN = "";
                    tv.STATUS_DELETE = 1;
                    db.THANHVIENs.InsertOnSubmit(tv);
                    db.SubmitChanges();
                    return RedirectToAction("QuanLyThanhVien", "Admin");
            }
            return RedirectToAction("QuanLyThanhVien", "Admin");
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
            if(don != null)
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
            return RedirectToAction("QuanLyThanhVien", "Admin");
        }


        public ActionResult DetailTv(int ma)
        {
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == ma);
            ViewBag.MATV = ma;
            ViewBag.HOTEN = tv.TENTHANHVIEN;
            ViewBag.GIOITINH =  tv.GIOITINH;
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
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == Convert.ToInt32(f["MATV"]));
            tv.TENTHANHVIEN = f["HOTEN"];
            tv.GIOITINH = f["GIOITINH"];
            tv.DIACHI = f["DIACHI"];
            tv.EMAIL = f["EMAIL"];
            tv.DIENTHOAI = f["DIENTHOAI"];
            tv.TAIKHOAN = f["TAIKHOAN"];
            tv.MATKHAU = f["MATKHAU"];
            tv.LINK_FACEBOOK = f["FACEBOOK"];
            //tv.ANH_DAI_DIEN = f[""];

            if(ANHDAIDIEN != null)
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
            return RedirectToAction("QuanLyThanhVien", "Admin");
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
            if(r != null)
            {
                db.ADMIN_REPLies.DeleteOnSubmit(r);
                db.SubmitChanges();
            }
            db.BINHLUANDANHGIAs.DeleteOnSubmit(b);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "Admin");
        }
        public ActionResult DetailDg(int ma, int maBds)
        {
            var b = db.BINHLUANDANHGIAs.SingleOrDefault(n => n.MABL == ma);
            ViewBag.MABL = ma;
            ViewBag.TENBDS = db.BDS.SingleOrDefault(n => n.MABDS == ma).TENBDS;
            ViewBag.HINHANH = db.BDS.SingleOrDefault(n => n.MABDS == ma).HINHANH;
            ViewBag.HOTEN = b.HOTEN;
            ViewBag.NOIDUNG = b.NOIDUNG;
            ViewBag.NGAYBL = b.NGAYBL;
            ViewBag.SOSAO = b.SOSAO;
            var p = db.ADMIN_REPLies.Where(n => n.MABL == ma && n.MABDS == maBds).Count();
            if(p > 0)
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
            return RedirectToAction("QuanLyDanhGia", "Admin");
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
            ADMIN_REPLY a = new ADMIN_REPLY();
            a.MABL = Convert.ToInt32(f["MABL"]);
            a.MABDS = Convert.ToInt32(f["MABDS"]);
            a.MANV = Convert.ToInt32(f["MANV"]);
            a.NOIDUNG = f["NOIDUNGPH"];
            a.NGAYTRALOI = DateTime.Now;
            db.ADMIN_REPLies.InsertOnSubmit(a);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "Admin");
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
                ViewBag.TENNV = db.NHANVIENs.SingleOrDefault(n => n.MANV ==  maNv).HOTEN;
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
            ADMIN_REPLY a = db.ADMIN_REPLies.SingleOrDefault(n => n.MAREPLY == Convert.ToInt32(f["MAREPLY"]));
            a.NOIDUNG = f["NOIDUNGPH"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyDanhGia", "Admin");
        }








        //Quản lý tin nhắn
        public ActionResult QuanLyTinNhan()
        {
            var t = db.TINNHAN_MAILs.ToList();
            return View(t);
        }

        public ActionResult DeleteTn(int ma)
        {
            foreach(var item in db.REPLY_TINNHAN_MAILs.Where(n => n.MATINNHAN == ma).ToList())
            {
                db.REPLY_TINNHAN_MAILs.DeleteOnSubmit(item);
                db.SubmitChanges();
            }
            var tn = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == ma);
            db.TINNHAN_MAILs.DeleteOnSubmit(tn);
            db.SubmitChanges();
            return RedirectToAction("QuanLyTinNhan", "Admin");
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
            if(s != null)
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
            var b = db.TINNHAN_MAILs.SingleOrDefault(n => n.MATINNHAN == Convert.ToInt32(f["MATN"]));
            b.TIEUDE = f["TIEUDE"];
            b.NOIDUNG = f["NOIDUNG"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyTinNhan", "Admin");
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
            message.To.Add(new MailAddress(b.EMAIL));
            message.Subject = f["TIEUDE"];
            message.Body = f["NOIDUNG"];
            mail.Send(message);
            return RedirectToAction("QuanLyTinNhan", "Admin");
        }




        //Quản lý đơn đặt cọc
        public ActionResult QuanLyDonDatCoc()
        {
            var d = db.CHITIETDONDATCOCs.Where(n => n.STATE_DELETE != 0).OrderByDescending(n => n.TINHTRANG_THANHTOAN).ToList();
            return View(d.ToList());
        }
        public ActionResult Status_ThanhToan(int ma)
        {
            var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            if(ct.TINHTRANG_THANHTOAN == 0)
            {
                ct.TINHTRANG_THANHTOAN = 1;
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "Admin");
            }
            else
            {
                ct.TINHTRANG_THANHTOAN = 0;
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "Admin");
            }
        }
        public ActionResult DeleteDon(int ma)
        {
            var ct = db.CHITIETDONDATCOCs.Where(n => n.MADON == ma).ToList();
            if(ct != null)
            {
                foreach(var item in ct)
                {
                    //db.CHITIETDONDATCOCs.DeleteOnSubmit(item);
                    item.STATE_DELETE = 0;
                    db.SubmitChanges();
                }
                //var d = db.DONDATCOCs.SingleOrDefault(n => n.MADON == ma);
                //db.DONDATCOCs.DeleteOnSubmit(d);
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "Admin");
            }
            else
            {
                return RedirectToAction("QuanLyDonDatCoc", "Admin");
            }

        }

        public ActionResult DetailDon(int ma)
        {
            var d = db.DONDATCOCs.SingleOrDefault(n => n.MADON ==ma);
            var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON ==ma);
            ViewBag.MADON = ma;
            ViewBag.HOTEN = ct.THANHVIEN.TENTHANHVIEN;
            ViewBag.NGAYDAT = d.NGAYDAT;
            ViewBag.DUYET = d.DUYET;
            ViewBag.MABDS = ct.MABDS;
            ViewBag.MALOAI = ct.MALOAI;
            ViewBag.TENLOAI = ct.LOAIBDS.TENLOAI;
            ViewBag.TENBDS = ct.BDS.TENBDS;
            ViewBag.TONGTIEN = ct.TONGTIEN;
            ViewBag.TINHTRANG_THANHTOAN = ct.TINHTRANG_THANHTOAN;
            return View();
        }

        [HttpGet]
        public ActionResult CreateDon()
        {
            ViewBag.MABDS = new SelectList(db.BDS.Where(n => n.STATE_DELETE != 0).ToList(), "MABDS", "TENBDS");
            ViewBag.MATV = new SelectList(db.THANHVIENs.Where(n => n.STATUS_DELETE != 0).ToList(), "MATV", "TENTHANHVIEN");
            return View();
        }
        [HttpPost]
        public ActionResult CreateDon(FormCollection f)
        {
            DONDATCOC d = new DONDATCOC();
            CHITIETDONDATCOC ct = new CHITIETDONDATCOC();
            var madon = db.DONDATCOCs.OrderByDescending(n => n.MADON).First();

            d.NGAYDAT = DateTime.Now;
            d.NGAYHT = null;
            d.DUYET = 0;
            db.DONDATCOCs.InsertOnSubmit(d);
            db.SubmitChanges();

            ct.MADON = Convert.ToInt32(madon.MADON + 1);
            ct.MATV = Convert.ToInt32(f["MATV"]);
            ct.MABDS = Convert.ToInt32(f["MABDS"]);
            ct.STATE_DELETE = 1;
            var l = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
            ct.MALOAI = l.MALOAI_BDS;
            ct.TONGTIEN = Convert.ToDecimal(l.GIA*l.COC_TRUOC/100);
            ct.TINHTRANG_THANHTOAN = 0;
            db.CHITIETDONDATCOCs.InsertOnSubmit(ct);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDonDatCoc", "Admin");
        }


        [HttpGet]
        public ActionResult EditDon(int ma)
        {
            var b = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == b.MATV);
            var bds = db.BDS.SingleOrDefault(n => n.MABDS == b.MABDS);
            ViewBag.MABDS = new SelectList(db.BDS.Where(n => n.STATE_DELETE != 0).ToList(), "MABDS", "TENBDS", bds.TENBDS);
            ViewBag.MATV = new SelectList(db.THANHVIENs.Where(n => n.STATUS_DELETE != 0).ToList(), "MATV", "TENTHANHVIEN", tv.TENTHANHVIEN);
            ViewBag.MADON = ma;
            //ViewBag.MABDS = b.MABDS;
            //ViewBag.HOTEN = b.THANHVIEN.TENTHANHVIEN;
            //ViewBag.TENBDS = b.BDS.TENBDS;
            ViewBag.TENLOAI = b.LOAIBDS.TENLOAI;
            ViewBag.TONGTIEN = b.TONGTIEN;
            ViewBag.TINHTRANG_THANHTOAN = b.TINHTRANG_THANHTOAN;
            ViewBag.NGAYDAT = b.DONDATCOC.NGAYDAT;
            ViewBag.DUYET = b.DONDATCOC.DUYET;
            

            return View();
        }
        [HttpPost]
        public ActionResult EditDon(FormCollection f)
        {
            var b = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == Convert.ToInt32(f["MADON"]));
            b.MATV = Convert.ToInt32(f["MATV"]);
            b.MABDS = Convert.ToInt32(f["MABDS"]);
            var l = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(f["MABDS"]));
            b.MALOAI = l.MALOAI_BDS;
            b.TONGTIEN = Convert.ToDecimal(l.GIA * l.COC_TRUOC / 100);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDonDatCoc", "Admin");
        }



        //In hoá đơn
        //In hoá đơn cho đơn hàng
        public ActionResult ExportToWord(int maDon, int maNv)
        {
            int ma = Convert.ToInt32(Request.QueryString["maNv"]);
            HoaDonViewModel data = (from hd in db.CHITIETDONDATCOCs
                                    join tv in db.THANHVIENs
                                    on hd.MATV equals tv.MATV
                                    //join nv in db.NHANVIENs
                                    //on hd.MaNV equals nv.MaNV
                                    join bds in db.BDS
                                    on hd.MABDS equals bds.MABDS
                                    join loai in db.LOAIBDS
                                    on bds.MALOAI_BDS equals loai.MALOAI
                                    where hd.MADON == maDon
                                    select new HoaDonViewModel()
                                    {
                                        MaHD = Convert.ToInt32(hd.MADON),
                                        HoTenTV = tv.TENTHANHVIEN,
                                        HoTenNV = db.NHANVIENs.SingleOrDefault(n => n.MANV == maNv).HOTEN,
                                        TENBDS = bds.TENBDS,
                                        MOTA = bds.THONGTIN,

                                        GIABAN = (double)bds.GIA * 1000000000,
                                        TEXT_GIA = NumberToText((double)bds.GIA * 1000000000, true),

                                        TongTien = (double)hd.TONGTIEN * 1000000000,
                                        TEXT_TOTAL = NumberToText((double)hd.TONGTIEN * 1000000000, true),

                                        DiaChi = tv.DIACHI,
                                        Email = tv.EMAIL,
                                        TENLOAI = loai.TENLOAI,
                                        NgayThanhToan = DateTime.Now,
                                        MaNV = ma,
                                        ChuSoHuu = bds.THANHVIEN.TENTHANHVIEN
                                    }).SingleOrDefault();
            Session["maDon"] = data.MaHD;
            Session["tennv"] = data.HoTenNV;
            return View(data);
        }
        public ActionResult ExportPDF(int t, int maDon, int maNv)
        {
            var d = db.DONDATCOCs.SingleOrDefault(n => n.MADON == maDon);/*Convert.ToInt32(Session["maDon"].ToString()));*/
            d.DUYET = 1;
            d.NGAYHT = DateTime.Now;
            db.SubmitChanges();
            return new ActionAsPdf("ExportToWord", new { @maDon = maDon, @maNv = maNv })
            {
                FileName = Server.MapPath("~/Admin/Content/HoaDon.pdf")
            };
        }
        public string NumberToText(double TongTien, bool suffix)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = TongTien.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;


            return result + (suffix ? " đồng" : "");
        }


    }
}