using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EstateSolution.Models;
using PagedList.Mvc;
using PagedList;

namespace EstateSolution.Controllers
{
    public class HomeController : Controller
    {
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
        public List<BDS> ListBatDongSan(int count)
        {
            return data.BDS.Where(n => n.DUYET != 0 && n.MATV > 0)/*.Take(count)*/.ToList();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShortDatePage()
        {
            return View(ListBatDongSan(8).OrderByDescending(s => s.NGAYDANG));
        }

        public ActionResult ShortMoneyPage()
        {
            return View(ListBatDongSan(8).OrderByDescending(s => s.GIA));
        }
        public ActionResult BDS_TYPE()
        {
            var r = data.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            return View(r);
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Contact(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            ViewBag.HOTEN = f["HOTEN"];
            ViewBag.EMAIL = f["EMAIL"];
            ViewBag.TIEUDE = f["TIEUDE"];
            ViewBag.NOIDUNG = f["NOIDUNG"];

            if (f["HOTEN"].Equals(""))
            {
                ViewBag.ErrorHOTEN = "Vui lòng nhập họ tên!";
                return View();
            }
            else if (f["HOTEN"].ToString().Length > 30)
            {
                ViewBag.ErrorHOTEN = "Họ tên không được vượt quá 30 kí tự!";
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
            else if (f["TIEUDE"].ToString().Length > 100)
            {
                ViewBag.ErrorTIEUDE = "Tiêu đề không được vượt quá 100 kí tự!";
                return View();
            }
            else if (f["NOIDUNG"].Equals(""))
            {
                ViewBag.ErrorNOIDUNG = "Vui lòng nhập nội dung!";
                return View();
            }
            else if (f["NOIDUNG"].ToString().Length > 2000)
            {
                ViewBag.ErrorNOIDUNG = "Nội dung không được vượt quá 2000 kí tự!";
                return View();
            }
            if (fFileUpload == null)
            {
                TINNHAN_MAIL tn = new TINNHAN_MAIL();
                tn.HOTEN = f["HOTEN"];
                tn.EMAIL = f["EMAIL"];
                tn.TIEUDE = f["TIEUDE"];
                tn.NOIDUNG = f["NOIDUNG"];
                tn.NGAYNHAN = DateTime.Now;
                //tn.HINHANH = sFileName;
                tn.TINHTRANG_PHANHOI = 0;
                data.TINNHAN_MAILs.InsertOnSubmit(tn);
                data.SubmitChanges();
                return RedirectToAction("ThongBaoContact", "Home");
            }
            else if (ModelState.IsValid)
            {
                TINNHAN_MAIL tn = new TINNHAN_MAIL();
                var sFileName = Path.GetFileName(fFileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/assets/img/img-contact"), sFileName);
                try
                {
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                catch (Exception)
                {
                    ViewBag.FileStatus = "Error while file uploading.";
                }
                tn.HOTEN = f["HOTEN"];
                tn.EMAIL = f["EMAIL"];
                tn.TIEUDE = f["TIEUDE"];
                tn.NOIDUNG = f["NOIDUNG"];
                tn.NGAYNHAN = DateTime.Now;
                tn.HINHANH = sFileName;
                tn.TINHTRANG_PHANHOI = 0;
                data.TINNHAN_MAILs.InsertOnSubmit(tn);
                data.SubmitChanges();
                return RedirectToAction("ThongBaoContact", "Home");

            }
            return View();
        }

        public ActionResult ThongBaoContact()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangTin()
        {
            if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
            {
                return Redirect("~/User/LoginPage?id=4&&tb=1");
            }
            else
            {
                ViewBag.MALOAI = new SelectList(data.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                var id = data.BDS.Where(n => n.STATE_DELETE != 0).OrderByDescending(n => n.MABDS).First();
                ViewBag.MABDSD = (id.MABDS + 1).ToString();
                return View();
            } 
        }
        [HttpPost]
        public ActionResult DangTin(FormCollection f, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (fileUpload == null && fileUpload2 == null && fileUpload3 == null)
            {
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
                    {
                        return Redirect("~/User/LoginPage?id=4&&tb=1");
                    }
                    else
                    {
                        THANHVIEN tv = (THANHVIEN)Session["TAIKHOAN"];
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
                        ViewBag.MALOAI = new SelectList(data.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                        var id = data.BDS.OrderByDescending(n => n.MABDS).First();
                        ViewBag.MABDSD = (id.MABDS + 1).ToString();
                        BDS b = new BDS();
                        b.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                        b.TENBDS = f["TENBDS"];
                        b.THONGTIN = f["THONGTIN"];
                        b.GIA = Convert.ToInt32(f["GIA"]);
                        b.COC_TRUOC = Convert.ToInt32(f["SOPHANTRAM"]);
                        b.MA_AD_DUYET = 1;
                        b.NGAYDANG = DateTime.Now;
                        b.DUYET = 0;
                        b.SAO = 0;
                        b.HINHANH = fileName;
                        b.MATV = 2;//tv.MATV;
                        b.STATE_DELETE = 1;
                        data.BDS.InsertOnSubmit(b);
                        data.SubmitChanges();

                        DACDIEM_BDS dd = new DACDIEM_BDS();
                        dd.MABDS = (id.MABDS + 1);
                        dd.MALOAI_BDS = Convert.ToInt32(f["MALOAI"]);
                        dd.DIACHI = f["DIACHI"];
                        dd.DIENTICH = f["DIENTICH"];
                        dd.SOPHONGNGU = Convert.ToInt32(f["SOPHONGNGU"]);
                        dd.SOPHONGTAM = Convert.ToInt32(f["SOPHONGTAM"]);
                        dd.SOGARA = Convert.ToInt32(f["SOGARA"]);
                        dd.PHAPLY = f["PHAPLY"];
                        data.DACDIEM_BDS.InsertOnSubmit(dd);
                        data.SubmitChanges();

                        HINH_ANH_BDS ha = new HINH_ANH_BDS();
                        ha.MABDS = (id.MABDS + 1);
                        ha.HINHANH = fileName;
                        data.HINH_ANH_BDS.InsertOnSubmit(ha);
                        data.SubmitChanges();

                        HINH_ANH_BDS ha2 = new HINH_ANH_BDS();
                        ha2.MABDS = (id.MABDS + 1);
                        ha2.HINHANH = fileName2;
                        data.HINH_ANH_BDS.InsertOnSubmit(ha2);
                        data.SubmitChanges();

                        HINH_ANH_BDS ha3 = new HINH_ANH_BDS();
                        ha3.MABDS = (id.MABDS + 1);
                        ha3.HINHANH = fileName3;
                        data.HINH_ANH_BDS.InsertOnSubmit(ha3);
                        data.SubmitChanges();

                        return RedirectToAction("ThongBaoDangTin", "Home");
                    }
                
                }
                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult ThongBaoDangTin()
        {
            return View();
        }
        public ActionResult Detail(int idBds, int state)
        {
            ViewBag.STATE = state;
            ViewBag.MABDS = idBds;
            var bds = data.BDS.SingleOrDefault(n => n.MABDS == idBds);
            var tenLoai = data.LOAIBDS.SingleOrDefault(n => n.MALOAI == (int)bds.MALOAI_BDS);
            ViewBag.TenLoaiBDS = tenLoai.TENLOAI.ToString();
            ViewBag.SoLuongDanhGia = data.BINHLUANDANHGIAs.Count(n => n.MABDS == idBds);
            var sao = (from bl in data.BINHLUANDANHGIAs where bl.MABDS == idBds select bl).Sum(n => n.SOSAO);
            if(sao == null)
            {
                ViewBag.TongDanhGia = 0;
            }
            else
            {
                ViewBag.TongDanhGia = (from bl in data.BINHLUANDANHGIAs where bl.MABDS == idBds select bl).Sum(n => n.SOSAO);
            }
            return View(bds);
        }
        public ActionResult ImagePage(int idBds)
        {
            var t = data.HINH_ANH_BDS.Where(s => s.MABDS == idBds).ToList();

            return View(t);
        }
        public ActionResult DacDiemPage(int idBds)
        {
            var ddbds = from s in data.DACDIEM_BDS
                        where s.MABDS == idBds
                        select s;
            return View(ddbds.Single());
        }
        public ActionResult ChuSoHuu(int idBds)
        {
            var ddbds = from s in data.BDS
                        where s.MABDS == idBds
                        select s;
            var r = data.BDS.SingleOrDefault(n => n.MABDS == idBds);
            var ss = data.THANHVIENs.SingleOrDefault(n => n.MATV == r.MATV);
            return View(ss);
        }
        public ActionResult XemThem()
        {
            var r = data.BDS.Where(n => n.DUYET != 0).Take(5);
            return View(r.ToList());
        }

        public ActionResult Loai_Bds_Page(/*string s,*/ int maLoai)
        {
            //if (s.Equals("date"))
            //{
            //    var r = data.BDS.Where(s => s.MALOAI_BDS == maLoai);
            //}
            var r = data.BDS.Where(s => s.STATE_DELETE != 0 && s.MALOAI_BDS == maLoai && s.DUYET != 0 && s.MATV > 0);
            var ss = data.LOAIBDS.SingleOrDefault(s => s.STATUS_DELETE != 0 && s.MALOAI == maLoai);
            ViewBag.TenLoai = ss.TENLOAI.ToString();
            ViewBag.MaLoai = maLoai;
            return View(r.ToList());
        }
        public List<BDS> ListLoaiBatDongSan(int id)
        {
            return data.BDS.Where(n => n.STATE_DELETE != 0 && n.MALOAI_BDS == id && n.DUYET != 0 && n.MATV > 0).ToList();
        }
        public ActionResult Loai_Bds_Page_ShortDate(string id)
        {
            var r = ListLoaiBatDongSan(Convert.ToInt32(id));
            return View(r.OrderByDescending(n => n.NGAYDANG));
        }
        public ActionResult Loai_Bds_Page_ShortMoney(string id)
        {
            var r = ListLoaiBatDongSan(Convert.ToInt32(id));
            return View(r.OrderByDescending(n => n.GIA));
        }



        //Bình luận - Đánh giá

        [HttpGet]
        public ActionResult BinhLuanPartial(int ma)
        {
            ViewBag.MaBDS = ma;
            return View();
        }
        [HttpPost]
        public ActionResult BinhLuanPartial(FormCollection f)
        {
            int matv = Convert.ToInt32(f["MaTv"]);
            string Email = f["Email"];
            string HoTen = f["HoTen"];
            string NoiDung = f["txtBinhLuan"];
            int MaBds = Convert.ToInt32(f["MaBds"]);
            int DanhGia = Convert.ToInt32(f["rating"]);
            BINHLUANDANHGIA bl = new BINHLUANDANHGIA();
            bl.MATV = matv;
            bl.HOTEN = HoTen;
            bl.NOIDUNG = NoiDung;
            bl.MABDS = MaBds;
            bl.NGAYBL = @DateTime.Now;
            bl.EMAIL = Email;
            bl.SOSAO = DanhGia;
            bl.STATUS_DELETE = 1;
            data.BINHLUANDANHGIAs.InsertOnSubmit(bl);
            data.SubmitChanges();
            UpdateTrungBinhDanhGia(MaBds);
            return RedirectToAction("Detail", "Home", new { idBds = MaBds, state = 0 });
        }
        private void UpdateTrungBinhDanhGia(int ma)
        {
            BDS sp = data.BDS.Single(n => n.MABDS == ma);
            int SoLuongDanhGia = data.BINHLUANDANHGIAs.Count(n => n.MABDS == ma);
            int TongDanhGia = (int)(from bl in data.BINHLUANDANHGIAs where bl.MABDS == ma select bl).Sum(n => n.SOSAO);
            if (SoLuongDanhGia != 0)
                sp.SAO = (TongDanhGia * 10 / SoLuongDanhGia) / 10;
            else
                sp.SAO = 0;
            data.SubmitChanges();
        }
        public List<BinhLuanItem> LayBinhLuan(int ma)
        {
            List<BinhLuanItem> lstBinhLuan = new List<BinhLuanItem>();
            var rs = (from bl in data.BINHLUANDANHGIAs where bl.MABDS == ma && bl.STATUS_DELETE != 0 select bl).OrderByDescending(n => n.NGAYBL).ToList();
            for (int i = 0; i < rs.Count(); i++)
            {
                lstBinhLuan.Add(new BinhLuanItem((int)rs[i].MABDS));
            }
            return lstBinhLuan;
        }
        public ActionResult ListBinhLuan(int ma, int? page)
        {
            int number = 5, iPageNum = (page ?? 1);
            ViewBag.MaBds = ma;
            var list = (from bl in data.BINHLUANDANHGIAs where bl.MABDS == ma && bl.STATUS_DELETE != 0 select bl).OrderByDescending(n => n.NGAYBL).ToList();
            if(list.Count == 0)
            {
                ViewBag.Tb = "Chưa có bình luận nào.";
            }
            else
            {
                return View(list.ToPagedList(iPageNum, number));
            }
            return View(list.ToPagedList(iPageNum, number));
        }
        public ActionResult ReplyCuaAdmin(int mabl)
        {
            ADMIN_REPLY a = data.ADMIN_REPLies.SingleOrDefault(n => n.MABL == mabl);
            
            if(a != null)
            {
                NHANVIEN nv = data.NHANVIENs.SingleOrDefault(n => n.MANV == a.MANV);
                ViewBag.TENNV = nv.HOTEN;
                ViewBag.NGAYTL = a.NGAYTRALOI;
                ViewBag.NOIDUNG = a.NOIDUNG;
                ViewBag.STATE = 1;
                //ViewBag.ANHDAIDIEN = nv.ANH_DAI_DIEN;
                return View(a);
            }
            else
            {
                ViewBag.STATE = 0;
                return View();
            }
        }




        public JsonResult getListDataType()
        {
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //if (id.Equals("shortDate"))
            //{
            //    var item = data.BDS.OrderBy(n => n.NGAYDANG).ToList();
            //    return Json(item, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var item = data.BDS.OrderBy(n => n.GIA).ToList();
            //    return Json(item, JsonRequestBehavior.AllowGet);
            //}
            List<BDS> item = new List<BDS>();
            item = data.BDS.OrderBy(n => n.NGAYDANG).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getListImage(string id)
        {
            List<HINH_ANH_BDS> item = new List<HINH_ANH_BDS>();
            item = data.HINH_ANH_BDS.Where(s => s.MABDS == Convert.ToInt16(id)).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}