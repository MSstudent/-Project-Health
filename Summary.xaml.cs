using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Health
{
    public partial class Summary : UserControl
    {
        private string text = "";
        private bool bShare = false;

        public bool IsShare
        {
            get
            {
                return bShare;
            }
        }

        public Summary(int Count)
        {
            InitializeComponent();
            text = string.Format("本次您共做了{0}个俯卧撑\n消耗了{1}卡路里\n", Count, Count * 400);
            TextBlock.Text = text;
        }

        private void ShareWeibo_Checked(object sender, RoutedEventArgs e)
        {
            bShare = true;
        }

        private void ShareWeibo_Unchecked(object sender, RoutedEventArgs e)
        {
            bShare = false;
        }
    }
}
