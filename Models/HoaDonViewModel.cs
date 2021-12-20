using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateSolution.Models
{
    public class HoaDonViewModel
    {
        public int MaHD { get; set; }

        public int? MaKH { get; set; }

        public int? MaNV { get; set; }

        public int? MaLoaiThucPham { get; set; }
        public int? MaThucPham { get; set; }

        public string MOTA { get; set; }
        public double? GIABAN { get; set; }
        public string TEXT_GIA { get; set; }
        public double? TongTien { get; set; }
        public string DIACHINGUOIBAN { get; set; }

        public DateTime? NgayThanhToan { get; set; }

        public double TongTienVAT
        {
            get
            {
                return (double)TongTien * 0.1 + (double)TongTien;
            }

        }

        public double VAT
        {
            get
            {
                return (double)TongTien * 0.1;
            }

        }

        public string HoTenTV { get; set; }

        public string HoTenNV { get; set; }
        public string ChuSoHuu { get; set; }

        public string TENBDS { get; set; }

        public string DiaChi { get; set; }

        public string Email { get; set; }

        public string TENLOAI { get; set; }
        public string TEXT_TOTAL { get; set; }

        
    }
}