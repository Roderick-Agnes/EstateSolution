using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateSolution.Models
{
    public class BinhLuanItem
    {
        dbBatDongSanDataContext data = new dbBatDongSanDataContext();
        public int maBds { get; set; }
        public int maTv { get; set; }
        public int maPq { get; set; }
        public int soSao { get; set; }
        public string hoTen { get; set; }
        public string noiDung { get; set; }
        public string email { get; set; }
        public string tenThanhVien { get; set; }
        public DateTime ngayBinhLuan { get; set; }
        public int status_delete { get; set; }

        public BinhLuanItem(int ma)
        {
            maBds = ma;
            BINHLUANDANHGIA bl = data.BINHLUANDANHGIAs.Single(n => n.MABDS == maBds);
            maTv = (int)bl.MATV;
            soSao = (int)bl.SOSAO;
            noiDung = bl.NOIDUNG;
            ngayBinhLuan = (DateTime)bl.NGAYBL;
            status_delete = (int)bl.STATUS_DELETE;

            THANHVIEN tv = data.THANHVIENs.Single(n => n.MATV == bl.MATV);
            hoTen = tv.TENTHANHVIEN;
            email = tv.EMAIL;


        }
    }
}