using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EstateSolution.Models
{
    public class MonthObject
    {
        [Key]
        string tenLoaiBds { get; set; }
        decimal tongDoanhThu { get; set; }
        
        public MonthObject(string tenLoai, decimal doanhThu)
        {
            this.tenLoaiBds = tenLoai;
            this.tongDoanhThu = doanhThu;
        }
    }
}