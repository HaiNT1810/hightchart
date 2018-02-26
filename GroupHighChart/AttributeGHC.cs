using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tandan.ISO.Webpart.GroupHighChart
{
    class AttributeGHC
    {
        
    }
    public class GetUser
    {
        public string[] HoVaTen { get; set; }
        public string[] User { get; set; }
    }
    public class SeriesPS
    {
        public string name { get; set; }
        public int[] data { get; set; }
    }
    //get data of a person
    public class GetDataPS
    {
        public int ID { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string NguoiThucHien { get; set; }
        public string DaKetThuc { get; set; }
        public string NguoiDaThucHien { get; set; }
        public GetDataPS(int id, DateTime ngayBatDau, string nguoiThucHien, string daKetThuc, string nguoiDaThucHien)
        {
            this.ID = id;
            this.NgayBatDau = ngayBatDau;
            this.NguoiThucHien = nguoiThucHien;
            this.DaKetThuc = daKetThuc;
            this.NguoiDaThucHien = nguoiDaThucHien;
        }
    }
}
