using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateSolution.Models
{
    public class CartLoai
    {
        public CartLoai(string tenLoai, int soLuongDon, double doanhThu)
        {
            this.tenLoai = tenLoai;
            this.soLuongDon = soLuongDon;
            this.doanhThu = doanhThu;
        }
        public string tenLoai { get; set; }
        public int soLuongDon { get; set; }
        public double doanhThu { get; set; }
    }
}