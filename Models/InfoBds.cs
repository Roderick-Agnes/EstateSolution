using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EstateSolution.Models;
namespace EstateSolution.Models
{
    public class InfoBds
    {
        dbBatDongSanDataContext db = new dbBatDongSanDataContext();
        public int mabds { get; set; }
        public string tenbds { get; set; }
        public string hinhanh { get; set; }
        public decimal gia { get; set; }
        public string dientich { get; set; }
        public int sophongngu { get; set; }
        public int sophongtam { get; set; }
        public int sogara { get; set; }
        public DateTime ngaydang { get; set; }
        public InfoBds(int mabds, string tenbds, string hinhanh, decimal gia, DateTime ngaydang, string dientich, int sophongngu, int sophongtam, int sogara)
        {
            this.mabds = mabds;
            this.tenbds = tenbds;
            this.hinhanh = hinhanh;
            this.gia = gia;
            this.ngaydang = ngaydang;
            this.dientich = dientich;
            this.sophongngu = sophongngu;
            this.sophongtam = sophongtam;
            this.sogara = sogara;
        }
        public string re(string dientich)
        {
            int i = 0;
            string tmpArea = "";
            while (i < dientich.Length)
            {

                if (dientich[i] == 77 || dientich[i] == 109 /*.Equals("m") || dientich[i].Equals("M")*/)
                {
                    if (dientich[i + 1] == 50/*.Equals("2")*/)
                    {
                        tmpArea += "m<sup>2</sup>";
                        i += 2;
                    }
                }
                else
                {
                    tmpArea += dientich[i];
                    i++;
                }
            }
            return tmpArea;
        }

    }

}
