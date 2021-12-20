using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EstateSolution.Models;

namespace EstateSolution.Controllers
{
    public class GioHangController : Controller
    {
        dbBatDongSanDataContext data = new dbBatDongSanDataContext();
        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //khoi tao gio hang rong
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Action tra ve view GioHang
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        //Them san pham vao gio
        public ActionResult ThemGioHang(int ma, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iMABDS == ma);
            if (sp == null)
            {
                sp = new GioHang(ma);
                lstGioHang.Add(sp);
            }
            else
            {
                //sp.iSoLuong++;
                return RedirectToAction("Detail", "Home", new { idBds = ma, state = 2 });

            }
            return RedirectToAction("Detail", "Home", new { idBds = ma, state  = 1});
        }
        //Tinh tong so luong
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
        //Tinh tong tien
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }


        //Xoa san pham khoi gio hang
        public ActionResult XoaSPKhoiGioHang(int ma)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMABDS == ma);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMABDS == ma);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("GioHang");
        }
        //Cap nhat gio hang
        //public ActionResult CapNhatGioHang(int ma, FormCollection f)
        //{
        //    List<GioHang> lstGioHang = LayGioHang();
        //    GioHang sp = lstGioHang.SingleOrDefault(n => n.iMABDS == ma);
        //    //neu ton tai thi cho sua so luong
        //    if (sp != null)
        //    {
        //        sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
        //    }
        //    return RedirectToAction("GioHang");
        //}
        //Xoa gio hang
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "Home"); //edit rong

        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
            {
                return Redirect("~/User/LoginPage?id=2&&tb=2");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home"); //edit rong
            }

            List<GioHang> lstGiohang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();

            var id = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
            if (id == null)
            {
                ViewBag.MaDonHang = 1;
            }
            else
            {
                ViewBag.MaDonHang = id.MADON+1;
            }
            
            ViewBag.TongTien = TongTien();

            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATCOC d = new DONDATCOC();

            THANHVIEN kh = (THANHVIEN)Session["TAIKHOAN"];
            List<GioHang> listGioHang = LayGioHang();
            if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
            {
                return Redirect("~/User/LoginPage?id=2&&tb=2");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home"); //edit rong
            }
            if (Session["TAIKHOAN"] != null && Session["GioHang"] != null)
            {
                double tien = 0;
                foreach (var item in listGioHang)///////////////bien doi
                {
                    var id = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
                    
                    d.NGAYDAT = DateTime.Now;
                    //var NgayGiao = String.Format("{0:MM/dd/yyyy}", Request.Form["NgayGiao"]);
                    //qldh.NgayGiao = DateTime.Parse(NgayGiao);
                    d.NGAYHT = null;
                    d.DUYET = 0;
                    data.DONDATCOCs.InsertOnSubmit(d);
                    data.SubmitChanges();


                    CHITIETDONDATCOC ct = new CHITIETDONDATCOC();
                    ct.MADON = id.MADON + 1;
                    ct.MABDS = item.iMABDS;
                    ct.MATV = kh.MATV;
                    ct.MALOAI = item.iMALOAI;
                    ct.TONGTIEN = (decimal)item.dThanhTien;
                    tien += item.dThanhTien;
                    ct.TINHTRANG_THANHTOAN = 0;
                    ct.STATE_DELETE = 1;
                    data.CHITIETDONDATCOCs.InsertOnSubmit(ct);
                    data.SubmitChanges();
                }

                kh.TENTHANHVIEN = f["ten"];
                kh.EMAIL = f["email"];
                kh.DIENTHOAI = f["dienthoai"];
                kh.DIACHI = f["diachi"];

                var id2 = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
                ViewBag.MaDonHang = id2.MADON.ToString();
                ViewBag.TongTien = tien.ToString();

                data.SubmitChanges();
                Session["GioHang"] = null;
                return RedirectToAction("ThongBaoDonHangThanhCong", "GioHang");
            }
            return View();
        }
        //[HttpGet]
        //public ActionResult DatCocNgay(int ma)
        //{
        //    if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
        //    {
        //        return Redirect("~/User/LoginPage?id=5&&tb=2");
        //    }

        //    List<GioHang> lstGioHang = LayGioHang();
        //    GioHang sp = lstGioHang.Find(n => n.iMABDS == ma);
        //    sp = new GioHang(ma);
        //    lstGioHang.Add(sp);
        //    ViewBag.TongSoLuong = TongSoLuong();

        //    var id = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
        //    if (id == null)
        //    {
        //        ViewBag.MaDonHang = 1;
        //    }
        //    else
        //    {
        //        ViewBag.MaDonHang = id.MADON + 1;
        //    }

        //    ViewBag.TongTien = TongTien();

        //    return View(lstGioHang);
        //}
        //[HttpPost]
        //public ActionResult DatCocNgay(FormCollection f,int ma)
        //{
        //    List<GioHang> lstGioHang = LayGioHang();
        //    GioHang sp = lstGioHang.Find(n => n.iMABDS == ma);
        //    sp = new GioHang(ma);
        //    lstGioHang.Add(sp);
        //    DONDATCOC d = new DONDATCOC();

        //    THANHVIEN kh = (THANHVIEN)Session["TAIKHOAN"];
        //    List<GioHang> listGioHang = LayGioHang();
        //    if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
        //    {
        //        return Redirect("~/User/LoginPage?id=5&&tb=2");
        //    }
        //    if (Session["TAIKHOAN"] != null && Session["GioHang"] != null)
        //    {
        //        double tien = 0;
        //        foreach (var item in listGioHang)///////////////bien doi
        //        {
        //            var id = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
                    
        //            d.NGAYDAT = DateTime.Now;
        //            //var NgayGiao = String.Format("{0:MM/dd/yyyy}", Request.Form["NgayGiao"]);
        //            //qldh.NgayGiao = DateTime.Parse(NgayGiao);
        //            d.NGAYHT = null;
        //            d.DUYET = 0;
        //            data.DONDATCOCs.InsertOnSubmit(d);
        //            data.SubmitChanges();


        //            CHITIETDONDATCOC ct = new CHITIETDONDATCOC();
        //            ct.MADON = id.MADON + 1;
        //            ct.MABDS = item.iMABDS;
        //            ct.MALOAI = item.iMALOAI;
        //            ct.MATV = kh.MATV;
        //            ct.TONGTIEN = (decimal)item.dThanhTien;
        //            tien += item.dThanhTien;
        //            ct.TINHTRANG_THANHTOAN = 0;
        //            data.CHITIETDONDATCOCs.InsertOnSubmit(ct);
        //            data.SubmitChanges();
        //        }

        //        kh.TENTHANHVIEN = f["ten"];
        //        kh.EMAIL = f["email"];
        //        kh.DIENTHOAI = f["dienthoai"];
        //        kh.DIACHI = f["diachi"];

        //        var id2 = data.DONDATCOCs.OrderByDescending(n => n.MADON).First();
        //        ViewBag.MaDonHang = id2.MADON.ToString();
        //        ViewBag.TongTien = tien.ToString();

        //        data.SubmitChanges();
        //        Session["GioHang"] = null;
        //        return RedirectToAction("ThongBaoDonHangThanhCong", "GioHang");
        //    }
        //    return View();
        //}
        public ActionResult ThongBaoDonHangThanhCong()
        {
            return View();
        }
        public ActionResult HuyDonHang()
        {
            return View();
        }
    }
}