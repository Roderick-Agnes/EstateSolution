using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EstateSolution.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string tenLoai, double doanhThu)
        {
            this.tenLoaiBds = tenLoai;
            this.tongDoanhThu = doanhThu;
        }
        [DataMember(Name = "label")]
        public string tenLoaiBds = "";
        [DataMember(Name = "y")]
        public Nullable<double> tongDoanhThu = null;
    }
    
}