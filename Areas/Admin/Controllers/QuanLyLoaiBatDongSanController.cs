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
    public class QuanLyLoaiBatDongSanController : Controller
    {
        // GET: Admin/QuanLyLoaiBatDongSan
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
            ViewBag.TENLOAI = f["TENLOAI"];
            if (f["TENLOAI"].Equals(""))
            {
                ViewBag.ErrorTENLOAI = "Tên loại bất động sản không được để trống!";
                return View();
            }else if (f["TENLOAI"].ToString().Length > 100)
            {
                ViewBag.ErrorTENLOAI = "Tên loại bất động sản không được vượt quá 100 kí tự!";
                return View();
            }
            LOAIBDS l = new LOAIBDS();
            l.TENLOAI = f["TENLOAI"];
            l.STATUS_DELETE = 1;
            db.LOAIBDS.InsertOnSubmit(l);
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "QuanLyLoaiBatDongSan");
        }
        public ActionResult DeleteLoaiBds(int ma)
        {
            var s = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == ma);
            //db.LOAIBDS.DeleteOnSubmit(s);
            s.STATUS_DELETE = 0;
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "QuanLyLoaiBatDongSan");
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
            ViewBag.TENLOAI = f["TENLOAI"];
            if (f["TENLOAI"].Equals(""))
            {
                ViewBag.ErrorTENLOAI = "Tên loại bất động sản không được để trống!";
                return View();
            }
            else if (f["TENLOAI"].ToString().Length > 100)
            {
                ViewBag.ErrorTENLOAI = "Tên loại bất động sản không được vượt quá 100 kí tự!";
                return View();
            }
            LOAIBDS l = db.LOAIBDS.SingleOrDefault(n => n.MALOAI == Convert.ToInt32(f["MALOAI"]));
            l.TENLOAI = f["TENLOAI"];
            db.SubmitChanges();
            return RedirectToAction("QuanLyLoaiBDS", "QuanLyLoaiBatDongSan");
        }
    }
}