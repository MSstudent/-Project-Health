using Alexis.WindowsPhone.Social;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Health
{
    public partial class MainPage : PhoneApplicationPage
    {
        private int iCounter = 0;
        private bool bDoing = false;

        private IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            LoadImage();
            LoadData();
        }

        private void Counter_DoubleTap(object sender, GestureEventArgs e)
        {
            if (bDoing)
            {
                bDoing = false;
                Tips.Text = "双击计数器以开始";
                //Do somethine
                Finished();
            }
            else
            {
                bDoing = true;
                Tips.Text = "双击计数器以结束";
                //Do something
            }
        }

        private void Finished()
        {
            UpdateData();

            MessagePrompt msgPrompt = new MessagePrompt();
            msgPrompt.Body = new Summary(iCounter);
            msgPrompt.IsAppBarVisible = false;
            msgPrompt.Completed += msgPrompt_Completed;
            msgPrompt.Show();

            //save iCounter
            Counter.Text = (iCounter = 0).ToString();

        }
        private void msgPrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                var msgPrompt = sender as MessagePrompt;
                var summary = msgPrompt.Body as Summary;

                if (summary.IsShare)
                    ShareToWeibo(summary.TextBlock.Text);

            }
        }
        private void ShareToWeibo(string statues)
        {
            App.CurrentSocialType = SocialType.Weibo;
            App.Statues = statues;
            App.ShareImage = LoadImage();
            bool isLogin = true;
            switch (App.CurrentSocialType)
            {
                case SocialType.Weibo:
                    if (!(SocialAPI.WeiboAccessToken == null || SocialAPI.WeiboAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Tencent:
                    if (!(SocialAPI.TencentAccessToken == null || SocialAPI.TencentAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Renren:
                    if (!(SocialAPI.RenrenAccessToken == null || SocialAPI.RenrenAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Douban:
                    break;
                case SocialType.Net:
                    break;
                case SocialType.Sohu:
                    break;
                default:
                    break;
            }
            if (isLogin)
            {
                NavigationService.Navigate(new Uri("/SocialLoginPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/SocialSendPage.xaml", UriKind.Relative));
            }
        }
        private void UpdateData()
        {
            string key = DateTime.Now.ToString("yyyy/MM/dd");

            if (App.Dict.ContainsKey(key))
                App.Dict[key] += iCounter;
            else
                App.Dict.Add(key, iCounter);

            SaveData();
        }
            
        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (bDoing)
                Counter.Text = (++iCounter).ToString();
            else
            {
                PhotoChooserTask photo = new PhotoChooserTask();
                photo.ShowCamera = true;

                photo.PixelHeight = 480;
                photo.PixelWidth = 480;

                photo.Completed += new EventHandler<PhotoResult>(photo_Completed);

                try
                {
                    photo.Show();
                }
                catch
                {
                    MessageBox.Show("打开相册失败");
                }
            }
        }

        private void photo_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Save
                Byte[] _imageBytes = new Byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Read(_imageBytes, 0, _imageBytes.Length);
                SaveImage(_imageBytes);

                LoadImage();
            }
        }

        private void SaveImage(byte[] _imageBytes)
        {
            string filePath = "kiss";
            using (var stream = isoFile.CreateFile(filePath))
            {
                stream.Write(_imageBytes, 0, _imageBytes.Length);
            }
        }
        private WriteableBitmap LoadImage()
        {
            string filePath = "kiss";
            if (!isoFile.FileExists(filePath))
            {
                var stream = isoFile.CreateFile(filePath);
                var sri = Application.GetResourceStream(new Uri("kiss.png", UriKind.Relative));

                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(sri.Stream);
                WriteableBitmap wb = new WriteableBitmap(bitmap);

                Extensions.SaveJpeg(wb, stream, wb.PixelWidth, wb.PixelHeight, 0, 100);

                stream.Close();
            }
               
            using (var imageStream = isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
            {
                WriteableBitmap imageSrc = PictureDecoder.DecodeJpeg(imageStream);
                Image.Source = imageSrc;
                return imageSrc;
            }
        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChartPage.xaml",UriKind.Relative));
        }

        private void SaveData()
        {
            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy/MM/dd";

            var isoFileStream = isoFile.OpenFile("Data", FileMode.Create, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(isoFileStream))
            {
                DateTime today = DateTime.Now;
                foreach (string key in App.Dict.Keys)
                {
                    DateTime date = Convert.ToDateTime(key, dtFormat);
                    if (DateTime.Compare(date.AddDays(7), today) == 1)
                    {
                        string str = key + "," + App.Dict[key];
                        writer.WriteLine(str);
                    }
                }
            }
        }

        private void LoadData()
        {
            if (isoFile.FileExists("Data"))
            {
                var isoFileSteram = isoFile.OpenFile("Data", FileMode.Open, FileAccess.Read);
                using (StreamReader reader = new StreamReader(isoFileSteram))
                {
                    string[] stringArray;
                    char[] charArray = new char[] { ',' };

                    string str = reader.ReadLine();
                    while (str != null)
                    {
                        stringArray = str.Split(charArray);
                        App.Dict.Add(stringArray[0],Convert.ToInt32(stringArray[1]));

                        str = reader.ReadLine();
                    }
                }
            }
        }
    }
}