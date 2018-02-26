using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace Tandan.ISO.Webpart.PersonalHighChart
{
    public partial class PersonalHighChartUserControl : UserControl
    {
        public string SiteUrl = string.Empty;
        public string ListName = string.Empty;
        public string CamlQuery = string.Empty;
        public Categories categories = new Categories();
        public string categoryStr = string.Empty;
        public List<Series> series = new List<Series>();
        public List<GetData> getData = new List<GetData>();
        public Series DangXuLy = new Series();
        public Series ChuyenXuLy = new Series();
        public Series DaXuLy = new Series();
        public string DangXuLyStr = string.Empty;
        public string ChuyenXuLyStr = string.Empty;
        public string DaXuLyStr = string.Empty;
        public int[] lstId;
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime today = DateTime.Now;
                int tuan = GetWeekOrderInYear(today);
                DateTime firstDay = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), tuan - 3);
                DateTime lastDay = GetLastDayOfWeek(DateTime.Now.Year.ToString(), tuan);
                categories.Tuan1 = "Tuần " + (tuan - 3);
                categories.Tuan2 = "Tuần " + (tuan - 2);
                categories.Tuan3 = "Tuần " + (tuan - 1);
                categories.Tuan4 = "Tuần " + (tuan);
                //categoryStr = ConvertObjectToJson(categories);
                string url = Tandan.Utilities.Utility.GetAbsoluteSiteUrl(this.SiteUrl);
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists["CongViec"];

                        SPQuery query = new SPQuery();

                        query.Query = string.Concat("<Where>",
                                                        "<And>",
                                                            "<Geq>",
                                                                "<FieldRef Name='NgayBatDau'/>",
                                                                "<Value IncludeTimeValue='FALSE' Type='DateTime'>" + firstDay.ToString("yyyy-MM-ddThh:mm:ssZ") + "</Value>",
                                                            "</Geq>",
                                                            "<Leq>",
                                                                "<FieldRef Name='NgayBatDau'/>",
                                                                "<Value IncludeTimeValue='FALSE' Type='DateTime'>" + lastDay.ToString("yyyy-MM-ddThh:mm:ssZ") + "</Value>",
                                                            "</Leq>",
                                                        "</And>",
                                                    "</Where>",
                                                    "<OrderBy><FieldRef Name='ID' Ascending='TRUE' /></OrderBy>");
                        query.ViewFields = string.Concat("<FieldRef Name='ID'/>",
                                                        "<FieldRef Name='NgayBatDau'/>",
                                                        "<FieldRef Name='NguoiThucHien'/>",
                                                        "<FieldRef Name='DaKetThuc'/>",
                                                        "<FieldRef Name='NguoiDaThucHien'/>");
                        query.ViewFieldsOnly = true;
                        SPListItemCollection items = list.GetItems(query);
                        if (items != null && items.Count != 0)
                        {
                            DataTable dtGetData = items.GetDataTable();
                            DangXuLy = GetDataByLinQ(tuan, "Đang xử lý", dtGetData, web.CurrentUser.ToString());
                            ChuyenXuLy = GetDataByLinQ(tuan, "Đã chuyển xử lý", dtGetData, web.CurrentUser.ToString());
                            DaXuLy = GetDataByLinQ(tuan, "Đã xử lý", dtGetData, web.CurrentUser.ToString());
                            DangXuLyStr = ConvertObjectToJson(DangXuLy);
                            ChuyenXuLyStr = ConvertObjectToJson(ChuyenXuLy);
                            DaXuLyStr = ConvertObjectToJson(DaXuLy);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TextBox1.Text = ex.ToString();
            }
        }

        private string ConvertObjectToJson(object obj)
        {
            JavaScriptSerializer result = new JavaScriptSerializer();
            return result.Serialize(obj);
        }

        /// <summary>
        /// lấy ra tuần theo ngày hiện tại
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekOrderInYear(DateTime time)
        {
            CultureInfo myCI = CultureInfo.CurrentCulture;
            System.Globalization.Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            return myCal.GetWeekOfYear(time, myCWR, myFirstDOW);
        }
        /// <summary>
        /// tìm ngày đầu tiên theo tuần trong năm
        /// </summary>
        /// <param name="nam"></param>
        /// <param name="tuan"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(string nam, int tuan)
        {
            --tuan;
            DateTime d1;
            DateTime.TryParse(nam.ToString() + "/01/01", out d1);
            d1 = d1.AddDays(7 * tuan);
            while (d1.DayOfWeek != DayOfWeek.Monday) d1 = d1.AddDays(-1);
            DateTime d2 = d1.AddDays(6);
            return d1;
        }
        /// <summary>
        /// tìm ngày cuối cùng theo tuần trong năm
        /// </summary>
        /// <param name="nam"></param>
        /// <param name="tuan"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(string nam, int tuan)
        {
            --tuan;
            DateTime d1;
            DateTime.TryParse(nam.ToString() + "/01/01", out d1);
            d1 = d1.AddDays(7 * tuan);
            while (d1.DayOfWeek != DayOfWeek.Monday) d1 = d1.AddDays(-1);
            DateTime d2 = d1.AddDays(6);
            return d2;
        }
        /// <summary>
        /// lấy dữ liệu từ tuần hiện tại tên tình trạng xử lý và datatable đã lấy từ camlquery
        /// </summary>
        /// <param name="tuan"></param>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Series GetDataByLinQ(int tuan, string name, DataTable dt, string currentUser)
        {
            Series data = new Series();
            DateTime firstDayOfWeek = new DateTime();
            DateTime lastDayOfWeek = new DateTime();
            List<int> count = new List<int>();
            switch (name)
            {
                case "Đang xử lý":
                    for (int i = tuan - 3; i <= tuan; i++)
                    {
                        firstDayOfWeek = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), i);
                        lastDayOfWeek = GetLastDayOfWeek(DateTime.Now.Year.ToString(), i);
                        int _countDangXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("NguoiThucHien")) && x.Field<string>("NguoiThucHien").Contains(currentUser) && x.Field<DateTime?>("NgayBatDau").HasValue && x.Field<DateTime>("NgayBatDau").Date >= firstDayOfWeek.Date && x.Field<DateTime>("NgayBatDau").Date <= lastDayOfWeek.Date).Count();
                        count.Add(_countDangXuLy);
                    }
                    data.name = "Đang xử lý";
                    data.data = count.ToArray();
                    break;
                case "Đã chuyển xử lý":
                    for (int i = tuan - 3; i <= tuan; i++)
                    {
                        firstDayOfWeek = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), i);
                        lastDayOfWeek = GetLastDayOfWeek(DateTime.Now.Year.ToString(), i);
                        int _countChuyenXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("DaKetThuc")) && x.Field<string>("DaKetThuc").Equals("0") && !string.IsNullOrEmpty(x.Field<string>("NguoiDaThucHien")) && x.Field<string>("NguoiDaThucHien").Contains(currentUser) && x.Field<DateTime?>("NgayBatDau").HasValue && x.Field<DateTime>("NgayBatDau").Date >= firstDayOfWeek.Date && x.Field<DateTime>("NgayBatDau").Date <= lastDayOfWeek.Date).Count();
                        count.Add(_countChuyenXuLy);
                    }
                    data.name = "Đã chuyển xử lý";
                    data.data = count.ToArray();
                    break;
                case "Đã xử lý":
                    for (int i = tuan - 3; i <= tuan; i++)
                    {
                        firstDayOfWeek = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), i);
                        lastDayOfWeek = GetLastDayOfWeek(DateTime.Now.Year.ToString(), i);
                        int _countDaXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("DaKetThuc")) && x.Field<string>("DaKetThuc").Equals("1") && !string.IsNullOrEmpty(x.Field<string>("NguoiDaThucHien")) && x.Field<string>("NguoiDaThucHien").Contains(currentUser) && x.Field<DateTime?>("NgayBatDau").HasValue && x.Field<DateTime>("NgayBatDau").Date >= firstDayOfWeek.Date && x.Field<DateTime>("NgayBatDau").Date <= lastDayOfWeek.Date).Count();
                        count.Add(_countDaXuLy);
                    }
                    data.name = "Đã xử lý";
                    data.data = count.ToArray();
                    break;
            }
            return data;
        }
    }
}
