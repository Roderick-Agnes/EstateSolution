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
    public class QuanLyBatDongSanController : Controller
    {
        // GET: Admin/QuanLyBatDongSan
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
        public ActionResult QuanLyBDS()
        {
            var s = db.BDS.OrderByDescending(n => n.MABDS).Where(m => m.LOAIBDS.STATUS_DELETE == 1).ToList();
            return View(s);
        }
        public ActionResult DuyetBds(int ma, int state, int manv)
        {
            var b = db.BDS.SingleOrDefault(n => n.MABDS == ma);
            if (state != 1)
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
            return RedirectToAction("QuanLyBds", "QuanLyBatDongSan");
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
            var list = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            var l = db.BDS.SingleOrDefault(n => n.MABDS == Convert.ToInt32(Convert.ToInt32(f["MABDS"])));
            ViewBag.MALOAI = new SelectList(db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList(), "MALOAI", "TENLOAI", l.MALOAI_BDS);
            if (f["TENBDS"].ToString().Equals(""))
            {
                ViewBag.ErrorTENBDS = "Tên bất động sản không được để trống!";
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["TENBDS"].ToString(), 100) == 2)
            {
                ViewBag.ErrorTENBDS = "Tên bất động sản không được vượt quá 100 kí tự!";
                return View();
            }
            else if (f["THONGTIN"].ToString().Equals(""))
            {
                ViewBag.ErrorTHONGTIN = "Thông tin bất động sản không được để trống!";
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["THONGTIN"].ToString(), 1500) == 2)
            {
                ViewBag.ErrorTHONGTIN = "Thông tin bất động sản không được vượt quá 1500 kí tự!";
                return View();
            }
            else if (f["GIA"].ToString().Equals(""))
            {
                ViewBag.ErrorGIA = "Giá không được để trống!";
                return View();
            }
            else if (Convert.ToInt32(f["GIA"]) < 0)
            {
                ViewBag.ErrorGIA = "Giá không được nhỏ hơn 0!";
                return View();
            }
            else if (f["COCTRUOC"].ToString().Equals(""))
            {
                ViewBag.ErrorCOCTRUOC = "Số phần trăm cọc trước không được để trống!";
                return View();
            }
            else if (isEmptyNumber(Convert.ToInt32(f["COCTRUOC"]), 0) == 2)
            {
                ViewBag.ErrorCOCTRUOC = "Số phần trăm cọc trước không được nhỏ hơn 0!";
                return View();
            }
            else if (f["DIACHI"].ToString().Equals(""))
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được để trống!";
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["DIACHI"].ToString(), 100) == 2)
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được vượt quá 100 kí tự!";
                return View();
            }
            else if (f["DIENTICH"].ToString().Equals(""))
            {
                ViewBag.ErrorDIENTICH = "Diện tích không được để trống!";
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["DIENTICH"].ToString(), 30) == 2)
            {
                ViewBag.ErrorDIENTICH = "Diện tích không được vượt quá 30 kí tự!";
                return View();
            }
            else if (f["SOPHONGNGU"].ToString().Equals(""))
            {
                ViewBag.ErrorSOPHONGNGU = "Số phòng ngủ không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOPHONGNGU"]) < 0)
            {
                ViewBag.ErrorSOPHONGNGU = "Số phòng ngủ phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["SOPHONGTAM"].ToString().Equals(""))
            {
                ViewBag.ErrorSOPHONGTAM = "Số phòng tắm không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOPHONGTAM"]) < 0)
            {
                ViewBag.ErrorSOPHONGTAM = "Số phòng tắm phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["SOGARA"].ToString().Equals(""))
            {
                ViewBag.ErrorSOGARA = "Số gara không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOGARA"]) < 0)
            {
                ViewBag.ErrorSOGARA = "Số gara phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["PHAPLY"].ToString().Equals(""))
            {
                ViewBag.ErrorPHAPLY = "Pháp lý không được để trống!";
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["PHAPLY"].ToString(), 50) == 2)
            {
                ViewBag.ErrorPHAPLY = "Pháp lý không được vượt quá 50 kí tự!";
                return View();
            }
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
                        if (i < newListHa.Length)
                        {
                            item.HINHANH = newListHa[i].ToString();// fileName.ToString();
                            db.SubmitChanges();
                        }

                    }
                    db.SubmitChanges();

                    return RedirectToAction("QuanLyBDS", "QuanLyBatDongSan");
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
            return RedirectToAction("QuanLyBDS", "QuanLyBatDongSan");
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
            var list = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            ViewBag.TENBDS = f["TENBDS"].ToString();
            ViewBag.THONGTIN = f["THONGTIN"].ToString();
            ViewBag.GIA = f["GIA"].ToString();
            ViewBag.COCTRUOC = f["COCTRUOC"].ToString();
            ViewBag.DIACHI = f["DIACHI"].ToString();
            ViewBag.DIENTICH = f["DIENTICH"].ToString();
            ViewBag.SOPHONGNGU = f["SOPHONGNGU"].ToString();
            ViewBag.SOPHONGTAM = f["SOPHONGTAM"].ToString();
            ViewBag.SOGARA = f["SOGARA"].ToString();
            ViewBag.PHAPLY = f["PHAPLY"].ToString();
            int idad = 2;
            ViewBag.MAAD = f["maad"].ToString();
            if (f["maad"].ToString().Equals(""))
            {
                idad = Convert.ToInt32(f["maad2"].ToString());
                ViewBag.MAAD = Convert.ToInt32(f["maad2"].ToString());
            }
            else
            {
                idad = Convert.ToInt32(f["maad"].ToString());
            }
            if (f["TENBDS"].ToString().Equals(""))
            {
                ViewBag.ErrorTENBDS = "Tên bất động sản không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["TENBDS"].ToString(), 100) == 2)
            {
                ViewBag.ErrorTENBDS = "Tên bất động sản không được vượt quá 100 kí tự!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["THONGTIN"].ToString().Equals(""))
            {
                ViewBag.ErrorTHONGTIN = "Thông tin bất động sản không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["THONGTIN"].ToString(), 1500) == 2)
            {
                ViewBag.ErrorTHONGTIN = "Thông tin bất động sản không được vượt quá 1500 kí tự!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["GIA"].ToString().Equals(""))
            {
                ViewBag.ErrorGIA = "Giá không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyNumber(Convert.ToInt32(f["GIA"]), 0) == 2)
            {
                ViewBag.ErrorGIA = "Giá không được nhỏ hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["COCTRUOC"].ToString().Equals(""))
            {
                ViewBag.ErrorCOCTRUOC = "Số phần trăm cọc trước không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyNumber(Convert.ToInt32(f["COCTRUOC"]), 0) == 2)
            {
                ViewBag.ErrorCOCTRUOC = "Số phần trăm cọc trước không được nhỏ hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["DIACHI"].ToString().Equals(""))
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["DIACHI"].ToString(), 100) == 2)
            {
                ViewBag.ErrorDIACHI = "Địa chỉ không được vượt quá 100 kí tự!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["DIENTICH"].ToString().Equals(""))
            {
                ViewBag.ErrorDIENTICH = "Diện tích không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["DIENTICH"].ToString(), 30) == 2)
            {
                ViewBag.ErrorDIENTICH = "Diện tích không được vượt quá 30 kí tự!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["SOPHONGNGU"].ToString().Equals(""))
            {
                ViewBag.ErrorSOPHONGNGU = "Số phòng ngủ không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOPHONGNGU"]) < 0)
            {
                ViewBag.ErrorSOPHONGNGU = "Số phòng ngủ phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["SOPHONGTAM"].ToString().Equals(""))
            {
                ViewBag.ErrorSOPHONGTAM = "Số phòng tắm không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOPHONGTAM"]) < 0)
            {
                ViewBag.ErrorSOPHONGTAM = "Số phòng tắm phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["SOGARA"].ToString().Equals(""))
            {
                ViewBag.ErrorSOGARA = "Số gara không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (Convert.ToInt32(f["SOGARA"]) < 0)
            {
                ViewBag.ErrorSOGARA = "Số gara phải lớn hơn 0!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (f["PHAPLY"].ToString().Equals(""))
            {
                ViewBag.ErrorPHAPLY = "Pháp lý không được để trống!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (isEmptyOrConditionNotMatch(f["PHAPLY"].ToString(), 50) == 2)
            {
                ViewBag.ErrorPHAPLY = "Pháp lý không được vượt quá 50 kí tự!";
                ViewBag.MALOAI = new SelectList(list.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (fileUpload == null && fileUpload2 == null && fileUpload3 == null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (fileUpload == null && fileUpload2 != null && fileUpload3 != null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (fileUpload != null && fileUpload2 == null && fileUpload3 != null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return View();
            }
            else if (fileUpload != null && fileUpload2 != null && fileUpload3 == null)
            {
                ViewBag.Tb = "Vui lòng upload đủ 3 hình ảnh";
                ViewBag.MALOAI = new SelectList(db.LOAIBDS.ToList(), "MALOAI", "TENLOAI");
                return View();
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

                    return RedirectToAction("QuanLyBDS", "QuanLyBatDongSan");
                }
                return View();
            }
        }

        public ActionResult DeleteBds(int ma)
        {
            List<BINHLUANDANHGIA> bl = db.BINHLUANDANHGIAs.Where(n => n.MABDS == ma).ToList();
            List<ADMIN_REPLY> adr = db.ADMIN_REPLies.Where(n => n.MABDS == ma).ToList();
            if (adr != null)
            {
                foreach (var item in adr)
                {
                    db.ADMIN_REPLies.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }

            }
            if (bl != null)
            {
                foreach (var item in bl)
                {
                    db.BINHLUANDANHGIAs.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }
            List<CHITIETDONDATCOC> ct = db.CHITIETDONDATCOCs.Where(n => n.MABDS == ma).ToList();
            if (ct != null)
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

            return RedirectToAction("QuanLyBds", "QuanLyBatDongSan");
        }
    }
}