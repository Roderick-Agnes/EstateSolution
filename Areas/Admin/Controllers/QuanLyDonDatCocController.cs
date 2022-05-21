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
    public class QuanLyDonDatCocController : Controller
    {
        // GET: Admin/QuanLyDonDatCoc
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

        //Quản lý đơn đặt cọc
        public ActionResult QuanLyDonDatCoc()
        {
            var d = db.CHITIETDONDATCOCs.Where(n => n.STATE_DELETE != 0).OrderByDescending(n => n.TINHTRANG_THANHTOAN).ToList();
            return View(d.ToList());
        }
        public ActionResult Status_ThanhToan(int ma)
        {
            var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            var ddc = db.DONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            if (ct.TINHTRANG_THANHTOAN == 0)
            {
                ct.TINHTRANG_THANHTOAN = 1;
                ddc.DUYET = 1;
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
            }
            else
            {
                ct.TINHTRANG_THANHTOAN = 0;
                ddc.DUYET = 0;
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
            }
        }
        public ActionResult DeleteDon(int ma)
        {
            var ct = db.CHITIETDONDATCOCs.Where(n => n.MADON == ma).ToList();
            if (ct != null)
            {
                foreach (var item in ct)
                {
                    //db.CHITIETDONDATCOCs.DeleteOnSubmit(item);
                    item.STATE_DELETE = 0;
                    db.SubmitChanges();
                }
                //var d = db.DONDATCOCs.SingleOrDefault(n => n.MADON == ma);
                //db.DONDATCOCs.DeleteOnSubmit(d);
                db.SubmitChanges();
                return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
            }
            else
            {
                return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
            }

        }

        public ActionResult DetailDon(int ma)
        {
            var d = db.DONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            var ct = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == ma);
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
            ct.TONGTIEN = Convert.ToDecimal(l.GIA * l.COC_TRUOC / 100);
            ct.TINHTRANG_THANHTOAN = 0;
            db.CHITIETDONDATCOCs.InsertOnSubmit(ct);
            db.SubmitChanges();
            return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
        }


        [HttpGet]
        public ActionResult EditDon(int ma)
        {
            var b = db.CHITIETDONDATCOCs.SingleOrDefault(n => n.MADON == ma);
            var tv = db.THANHVIENs.SingleOrDefault(n => n.MATV == b.MATV);
            var bds = db.BDS.SingleOrDefault(n => n.MABDS == b.MABDS);
            ViewBag.MABDS = b.MABDS;
            ViewBag.TENBDS = bds.TENBDS;
            ViewBag.MATV = b.MATV;
            ViewBag.MADON = ma;
            //ViewBag.MABDS = b.MABDS;
            ViewBag.TENTHANHVIEN = tv.TENTHANHVIEN;
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
            return RedirectToAction("QuanLyDonDatCoc", "QuanLyDonDatCoc");
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