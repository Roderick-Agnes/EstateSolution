using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EstateSolution.Models;
using PagedList;

namespace EstateSolution.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        dbBatDongSanDataContext db = new dbBatDongSanDataContext();
        [HttpPost]
        public ActionResult SearchPage(FormCollection f, int? page)
        {
            string sTuKhoa = "";
            string sTypeBds = "";
            if (f["txtTimKiem"].ToString() != null && f["typeBds"].ToString() != null)
            {
                sTuKhoa = f["txtTimKiem"].ToString();
                sTypeBds = f["typeBds"].ToString();
                ViewBag.TuKhoa = "Từ khóa: " + sTuKhoa + " - " + sTypeBds;
                List<InfoBds> listDatas = new List<InfoBds>();
                int ids;
                List<BDS> lstKQTK = db.BDS.Where(n => n.STATE_DELETE != 0 && n.TENBDS.Contains(sTuKhoa) || n.LOAIBDS.TENLOAI.Equals(sTypeBds) || n.THONGTIN.Contains(sTuKhoa) || n.GIA.ToString().Contains(sTuKhoa)).ToList();
                for (int i = 0; i < lstKQTK.Count(); i++)
                {
                    ids = lstKQTK[i].MABDS;
                    var dataDacDiem = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == ids);
                    //add vào list
                    listDatas.Add(new InfoBds(lstKQTK[i].MABDS, lstKQTK[i].TENBDS.ToString(), lstKQTK[i].HINHANH.ToString(), (decimal)lstKQTK[i].GIA, (DateTime)lstKQTK[i].NGAYDANG, dataDacDiem.DIENTICH.ToString(), (int)dataDacDiem.SOPHONGNGU, (int)dataDacDiem.SOPHONGTAM, (int)dataDacDiem.SOGARA));
                }
                //Phân trang
                int pageNumbers = (page ?? 1);
                int pageSizes = 9;
                ViewBag.ThongBao = "Đã tìm thấy " + listDatas.Count + " kết quả!";
                return View(listDatas.ToPagedList(pageNumbers, pageSizes));
            }
            else if (f["txtTimKiem"].ToString() != null && f["typeBds"].ToString() == null)
            {
                sTuKhoa = f["txtTimKiem"].ToString();
                ViewBag.TuKhoa = "Từ khóa: " + sTuKhoa;
                List<InfoBds> listData = new List<InfoBds>();
                int id;
                List<BDS> lstKQTK = db.BDS.Where(n => n.STATE_DELETE != 0 && n.THONGTIN.Contains(sTuKhoa) || n.TENBDS.Contains(sTuKhoa) || n.GIA.ToString().Contains(sTuKhoa)).ToList();
                for (int i = 0; i < lstKQTK.Count(); i++)
                {
                    id = lstKQTK[i].MABDS;
                    var dataDacDiem = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == id);
                    //add vào list
                    listData.Add(new InfoBds(lstKQTK[i].MABDS, lstKQTK[i].TENBDS.ToString(), lstKQTK[i].HINHANH.ToString(), (decimal)lstKQTK[i].GIA, (DateTime)lstKQTK[i].NGAYDANG, dataDacDiem.DIENTICH.ToString(), (int)dataDacDiem.SOPHONGNGU, (int)dataDacDiem.SOPHONGTAM, (int)dataDacDiem.SOGARA));
                }
                //Phân trang
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                if (listData.Count == 0)
                {
                    ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                }
                ViewBag.ThongBao = "Đã tìm thấy " + listData.Count + " kết quả!";
                return View(listData.ToPagedList(pageNumber, pageSize));
            }
            else if (f["txtTimKiem"].ToString() == null || f["typeBds"].ToString() != null)
            {
                sTypeBds = f["typeBds"].ToString();
                ViewBag.TuKhoa = "Từ khóa: " + sTypeBds;
                List<InfoBds> listData = new List<InfoBds>();
                int id;
                List<BDS> lstKQTK = db.BDS.Where(n => n.STATE_DELETE != 0 && n.LOAIBDS.TENLOAI.Equals(sTypeBds) || n.THONGTIN.Contains(sTypeBds)).ToList();
                for (int i = 0; i < lstKQTK.Count(); i++)
                {
                    id = lstKQTK[i].MABDS;
                    var dataDacDiem = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == id);
                    //add vào list
                    listData.Add(new InfoBds(lstKQTK[i].MABDS, lstKQTK[i].TENBDS.ToString(), lstKQTK[i].HINHANH.ToString(), (decimal)lstKQTK[i].GIA, (DateTime)lstKQTK[i].NGAYDANG, dataDacDiem.DIENTICH.ToString(), (int)dataDacDiem.SOPHONGNGU, (int)dataDacDiem.SOPHONGTAM, (int)dataDacDiem.SOGARA));
                }
                //Phân trang
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                if (listData.Count == 0)
                {
                    ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                }
                ViewBag.ThongBao = "Đã tìm thấy " + listData.Count + " kết quả!";
                return View(listData.ToPagedList(pageNumber, pageSize));
            }
            else if (f["txtTimKiem"].ToString() == null && f["typeBds"].ToString() == null)
            {
                ViewBag.TuKhoa = "Từ khóa: " + "null";
                List<InfoBds> listDatas = new List<InfoBds>();
                int ids;
                List<BDS> lstKQTKs = db.BDS.ToList();
                for (int i = 0; i < lstKQTKs.Count(); i++)
                {
                    ids = lstKQTKs[i].MABDS;
                    var dataDacDiem = db.DACDIEM_BDS.SingleOrDefault(n => n.MABDS == ids);
                    //add vào list
                    listDatas.Add(new InfoBds(lstKQTKs[i].MABDS, lstKQTKs[i].TENBDS.ToString(), lstKQTKs[i].HINHANH.ToString(), (decimal)lstKQTKs[i].GIA, (DateTime)lstKQTKs[i].NGAYDANG, dataDacDiem.DIENTICH.ToString(), (int)dataDacDiem.SOPHONGNGU, (int)dataDacDiem.SOPHONGTAM, (int)dataDacDiem.SOGARA));
                }
                //Phân trang
                int pageNumbers = (page ?? 1);
                int pageSizes = 9;
                ViewBag.ThongBao = "Đã tìm thấy " + listDatas.Count + " kết quả!";
                return View(listDatas.ToPagedList(pageNumbers, pageSizes));
            }

            return View();
        }

        public ActionResult SearchRcm()
        {
            var s = db.LOAIBDS.Where(n => n.STATUS_DELETE != 0).ToList();
            return View(s);
        }
    }
}

