<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupHighChartUserControl.ascx.cs" Inherits="Tandan.ISO.Webpart.GroupHighChart.GroupHighChartUserControl" %>

<script type="text/javascript" src="/_layouts/Tandan.ISO/Highcharts/highcharts.js"></script>
<script type="text/javascript" src="/_layouts/Tandan.ISO/Highcharts/exporting.js"></script>

<div id="GroupHighChart"></div>
<%--<asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" ></asp:TextBox>--%>
<script type="text/javascript">
    if ('<%=DangXuLyStr %>' != '' && '<%=ChuyenXuLyStr %>' != '' && '<%=DaXuLyStr %>' != '' && '<%=category %>' != '') {
        var category = JSON.parse('<%=category %>');
        var d = new Date();
        var n = d.getFullYear();
        Highcharts.chart('GroupHighChart', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Biểu đồ lượng công việc trong tháng của từng người trong phòng'
            },
            xAxis: {
                categories: category
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Công việc trong tháng'
                },
                stackLabels: {
                    enabled: true,
                    style: {
                        fontWeight: 'bold',
                        color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                    }
                }
            },
            legend: {
                align: 'right',
                x: -30,
                verticalAlign: 'top',
                y: 25,
                floating: true,
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                shadow: false
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Tổng: {point.stackTotal}'
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    dataLabels: {
                        enabled: true,
                        color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
                    }
                }
            },
            series: [JSON.parse('<%=DangXuLyStr %>'), JSON.parse('<%=ChuyenXuLyStr %>'), JSON.parse('<%=DaXuLyStr %>')]
        });
    }
    else $("#GroupHighChart").html("Chưa có dữ liệu");
</script>