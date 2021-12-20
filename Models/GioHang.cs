using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateSolution.Models
{
    public class GioHang
    {
        dbBatDongSanDataContext data = new dbBatDongSanDataContext();
        public int iMABDS { get; set; }
        public int iMALOAI { get; set; }
        public string sTENBDS { get; set; }
        public double dGIA { get; set; }
        public int dCOCTRUOC { get; set; }
        public int iMAADDUYET { get; set; }
        public DateTime dNGAYDANG { get; set; }
        public float iSAO { get; set; }
        public string sHINHANH { get; set; }
        public int iMATV { get; set; }
        public string sDIACHI { get; set; }
        public string sDIENTICH { get; set; }
        public int iSOPHONGNGU { get; set; }
        public int iSOPHONGTAM { get; set; }
        public int iSOGARA { get; set; }
        public string sPHAPLY { get; set; }
        public int iSoLuong { get; set; }
        public int ccheck { get; set; }
        /// <summary>
        /// Thanh toán bằng ngân lượng
        /// </summary>
        public double dThanhTien
        {
            get { return iSoLuong * (dGIA*dCOCTRUOC/100); }
        }

        public GioHang(int ma)
        {
            iMABDS = ma;
            BDS s = data.BDS.Single(n => n.MABDS == iMABDS);
            iMALOAI = Convert.ToInt32(s.MALOAI_BDS);
            sTENBDS = s.TENBDS;
            dGIA = Convert.ToDouble(s.GIA);
            dCOCTRUOC = Convert.ToInt32(s.COC_TRUOC);
            iMAADDUYET = Convert.ToInt32(s.MA_AD_DUYET);
            dNGAYDANG = Convert.ToDateTime(s.NGAYDANG);
            iSAO = Convert.ToInt32(s.SAO);
            sHINHANH = s.HINHANH.ToString();
            iMATV = Convert.ToInt32(s.MATV);
            iSoLuong = 1;

            DACDIEM_BDS d = data.DACDIEM_BDS.Single(m => m.MABDS == iMABDS);
            sDIACHI = d.DIACHI;
            sDIENTICH = d.DIENTICH;
            iSOPHONGNGU = Convert.ToInt32(d.SOPHONGNGU);
            iSOPHONGTAM = Convert.ToInt32(d.SOPHONGTAM);
            iSOGARA = Convert.ToInt32(d.SOGARA);
            sPHAPLY = d.PHAPLY;
        }
    }
}