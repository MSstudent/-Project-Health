using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Visifire.Charts;

namespace Health
{
    public partial class ChartPage : PhoneApplicationPage
    {
        public ChartPage()
        {
            InitializeComponent();
            DateBlock.Text = DateTime.Now.ToLongDateString();

            DrawChart();
        }

        private void DrawChart()
        {
            DataSeries dataSeries = new DataSeries();

            DateTime today = DateTime.Now;
            for (int i = -6; i <= 0; i++)
            {
                DateTime dt = today.AddDays(i);
                string date = dt.ToString("M-dd");
                string key = dt.ToString("yyyy/MM/dd");
                if (App.Dict.ContainsKey(key))
                {
                    dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = date, YValue = App.Dict[key], XValue = i + 6 });
                }
                else
                {
                    dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = date, YValue = 0, XValue = i + 6 });
                }
            }

            chart.Series.Add(dataSeries);
        }
    }
}