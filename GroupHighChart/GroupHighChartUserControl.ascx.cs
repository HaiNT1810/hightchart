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

namespace Tandan.ISO.Webpart.GroupHighChart
{
    public partial class GroupHighChartUserControl : UserControl
    {
        public string SiteUrl = string.Empty;
        public string ListName = string.Empty;
        public string CamlQuery = string.Empty;
        //public Categories categories = new Categories();
        public string category = string.Empty;
        public GetUser getUser = new GetUser();
        public List<SeriesPS> series = new List<SeriesPS>();
        public List<GetDataPS> getData = new List<GetDataPS>();
        public SeriesPS DangXuLy = new SeriesPS();
        public SeriesPS ChuyenXuLy = new SeriesPS();
        public SeriesPS DaXuLy = new SeriesPS();
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
                DateTime firstDay = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), tuan);
                DateTime lastDay = GetLastDayOfWeek(DateTime.Now.Year.ToString(), tuan);
                DateTime now = DateTime.Now;
                DateTime fd = new DateTime(now.Year, now.Month, 1);
                DateTime ld = fd.AddMonths(1).AddDays(-1);
                string[] user;
                //categories.Tuan1 = "Tuần " + (tuan - 3);
                //categories.Tuan2 = "Tuần " + (tuan - 2);
                //categories.Tuan3 = "Tuần " + (tuan - 1);
                //categories.Tuan4 = "Tuần " + (tuan);
                //categoryStr = ConvertObjectToJson(categories);
                string url = Tandan.Utilities.Utility.GetAbsoluteSiteUrl(this.SiteUrl);
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists["CongViec"];
                        SPQuery query = new SPQuery();
                        query.Query = string.Concat("<Where>",
                                                        "<Or>",
                                                            "<IsNull>",
                                                                "<FieldRef Name='HanHoanThanh'/>",
                                                            "</IsNull>",
                                                            "<And>",
                                                                "<IsNotNull>",
                                                                    "<FieldRef Name='HanHoanThanh'/>",
                                                                "</IsNotNull>",
                                                                "<Geq>",
                                                                    "<FieldRef Name='HanHoanThanh'/>",
                                                                    "<Value IncludeTimeValue='FALSE' Type='DateTime'>" + fd.ToString("yyyy-MM-ddThh:mm:ssZ") + "</Value>",
                                                                "</Geq>",
                                                            "</And>",
                                                        "</Or>",
                                                    "</Where>",
                                                    "<OrderBy><FieldRef Name='ID' Ascending='TRUE' /></OrderBy>");
                        query.ViewFields = string.Concat("<FieldRef Name='ID'/>",
                                                        "<FieldRef Name='NgayBatDau'/>",
                                                        "<FieldRef Name='NguoiThucHien'/>",
                                                        "<FieldRef Name='DaKetThuc'/>",
                                                        "<FieldRef Name='HanHoanThanh'/>",
                                                        "<FieldRef Name='NgayKetThuc'/>",
                                                        "<FieldRef Name='NgayChuyenXuLy'/>",
                                                        "<FieldRef Name='NguoiChuyenXuLy'/>",
                                                        "<FieldRef Name='NguoiDaThucHien'/>");
                        query.ViewFieldsOnly = true;
                        getUser = GetUser(web.CurrentUser.ToString());
                        //category = getUser.HoVaTen;
                        category = ConvertObjectToJson(getUser.HoVaTen);
                        SPListItemCollection items = list.GetItems(query);
                        if (items != null && items.Count != 0)
                        {
                            DataTable dtGetData = items.GetDataTable();
                            user = getUser.User;
                            DangXuLy = GetDataByLinQ(tuan, "Đang xử lý", dtGetData, user);
                            ChuyenXuLy = GetDataByLinQ(tuan, "Đã chuyển xử lý", dtGetData, user);
                            DaXuLy = GetDataByLinQ(tuan, "Đã xử lý", dtGetData, user);
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
        private GetUser GetUser(string currentUser)
        {
            GetUser result = new GetUser();
            try
            {
                string url = SPContext.Current.Site.Url;
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        List<string> phongBan = new List<string>();
                        string pb = string.Empty;
                        SPList list = web.Lists["NguoiDung"];
                        SPQuery query = new SPQuery();
                        query.Query = "<OrderBy><FieldRef Name='ID' Ascending='TRUE' /></OrderBy>";
                        query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='User'/><FieldRef Name='NhomNguoiDung'/><FieldRef Name='HoVaTen'/>";
                        query.ViewFieldsOnly = true;
                        DataTable dtGetData = list.GetItems(query).GetDataTable();
                        var _node = dtGetData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("User")) && x.Field<string>("User").Equals(currentUser) && !string.IsNullOrEmpty(x.Field<string>("NhomNguoiDung")));
                        //count.Add(_countDangXuLy);
                        if (_node.Any())
                        {
                            DataTable temp = _node.CopyToDataTable();
                            phongBan = temp.AsEnumerable().Select(x => x.Field<string>("NhomNguoiDung")).ToList();
                            for (int i = 0; i < phongBan.ToArray().Length; i++)
                            {
                                pb = phongBan.ToArray()[i];
                            }
                        }
                        if (!string.IsNullOrEmpty(pb))
                        {
                            var _temp = dtGetData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("NhomNguoiDung")) && x.Field<string>("NhomNguoiDung").Equals(pb));
                            if (_temp.Any())
                            {
                                DataTable totemp = _temp.CopyToDataTable();
                                result.User = totemp.AsEnumerable().Select(x => x.Field<string>("User")).ToArray();
                                result.HoVaTen = totemp.AsEnumerable().Select(x => x.Field<string>("HoVaTen")).ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
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
        public static SeriesPS GetDataByLinQ(int tuan, string name, DataTable dt, string[] user)
        {
            SeriesPS data = new SeriesPS();
            //DateTime firstDayOfWeek = GetFirstDayOfWeek(DateTime.Now.Year.ToString(), tuan);
            //DateTime lastDayOfWeek = GetLastDayOfWeek(DateTime.Now.Year.ToString(), tuan);
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<int> count = new List<int>();
            switch (name)
            {
                case "Đang xử lý":
                    for (int i = 0; i < user.Length; i++)
                    {
                        int _countDangXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("NguoiThucHien")) && x.Field<string>("NguoiThucHien").Contains(user[i]) && ((!x.Field<DateTime?>("HanHoanThanh").HasValue) || (x.Field<DateTime?>("HanHoanThanh").HasValue && x.Field<DateTime>("HanHoanThanh").Date >= firstDayOfMonth.Date))).Count();
                        count.Add(_countDangXuLy);
                    }
                    data.name = "Đang xử lý";
                    data.data = count.ToArray();
                    break;
                case "Đã chuyển xử lý":
                    for (int i = 0; i < user.Length; i++)
                    {
                        int _countChuyenXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("DaKetThuc")) && x.Field<string>("DaKetThuc").Equals("0") && !string.IsNullOrEmpty(x.Field<string>("NguoiChuyenXuLy")) && x.Field<string>("NguoiChuyenXuLy").Contains(user[i]) && (x.Field<DateTime?>("NgayChuyenXuLy").HasValue && x.Field<DateTime>("NgayChuyenXuLy").Date >= firstDayOfMonth.Date && x.Field<DateTime>("NgayChuyenXuLy").Date <= lastDayOfMonth.Date)).Count();
                        count.Add(_countChuyenXuLy);
                    }
                    data.name = "Đã chuyển xử lý";
                    data.data = count.ToArray();
                    break;
                case "Đã xử lý":
                    for (int i = 0; i < user.Length; i++)
                    {
                        int _countDaXuLy = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("DaKetThuc")) && x.Field<string>("DaKetThuc").Equals("1") && !string.IsNullOrEmpty(x.Field<string>("NguoiDaThucHien")) && x.Field<string>("NguoiDaThucHien").Contains(user[i]) && (x.Field<DateTime?>("NgayKetThuc").HasValue && x.Field<DateTime>("NgayKetThuc").Date >= firstDayOfMonth.Date && x.Field<DateTime>("NgayKetThuc").Date <= lastDayOfMonth.Date)).Count();
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
