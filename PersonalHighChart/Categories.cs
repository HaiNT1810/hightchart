using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tandan.ISO.Webpart.PersonalHighChart
{
    public class Categories
    {
        public string Tuan1 { get; set; }
        public string Tuan2 { get; set; }
        public string Tuan3 { get; set; }
        public string Tuan4 { get; set; }
    }
    public class Series
    {
        public string name { get; set; }
        public int[] data { get; set; }
    }
    public class GetData
    {
        public int ID { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string NguoiThucHien { get; set; }
        public string DaKetThuc { get; set; }
        public string NguoiDaThucHien { get; set; }
        public GetData(int id, DateTime ngayBatDau, string nguoiThucHien, string daKetThuc, string nguoiDaThucHien)
        {
            this.ID = id;
            this.NgayBatDau = ngayBatDau;
            this.NguoiThucHien = nguoiThucHien;
            this.DaKetThuc = daKetThuc;
            this.NguoiDaThucHien = nguoiDaThucHien;
        }
    }
}

