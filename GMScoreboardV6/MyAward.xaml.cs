using GMScoreboardV6.Models;
using GMScoreboardV6.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GMScoreboardV6
{
    /// <summary>
    /// MyAward.xaml 的交互逻辑
    /// </summary>
    public partial class MyAward : Window
    {
        public MyAward()
        {
            InitializeComponent();
        }

        string qq = string.Empty;
        BitmapImage closebtn = new BitmapImage();
        BitmapImage closeBtnHover = new BitmapImage();

        public MyAward(string _qq) : this()
        {
            qq = _qq;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var myPrizes = MyAwardMng.GetMyPrizes(qq);

            SetMyPrizes(myPrizes.body.prizes);

            try
            {
                closebtn = new BitmapImage();
                closebtn.BeginInit();
                closebtn.UriSource = new Uri("pack://application:,,,/GMScoreboardV6;component/ResUI/closebtnNoPrize.png");
                closebtn.EndInit();
                closeBtnHover = new BitmapImage();
                closeBtnHover.BeginInit();
                closeBtnHover.UriSource = new Uri("pack://application:,,,/GMScoreboardV6;component/ResUI/closebtnHover.png");
                closeBtnHover.EndInit();

            }
            catch (Exception exception)
            {
                BitmapImage noPrize = new BitmapImage();
                noPrize.BeginInit();
                noPrize.UriSource = new Uri("pack://application:,,,/GMScoreboardV6;component/ResUI/closebtnNoPrize.png");
                noPrize.EndInit();

                closebtn = noPrize;
                closeBtnHover = noPrize;
            }
        }

        void VisibleNoAward(bool isVisable)
        {
            this.Dispatcher.Invoke(new Action<bool>((visable) =>
            {
                imgNoPrize.Visibility = visable ? Visibility.Visible : Visibility.Collapsed;
                txtNoPrize.Visibility= visable ? Visibility.Visible : Visibility.Collapsed;
            }), isVisable);
        }

        void SetMyPrizes(List<Prizes> myPrizes)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                prizeBox.ItemsSource = myPrizes;
            }));
            VisibleNoAward(myPrizes.Count == 0);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (closeBtnHover==null)
                {
                    return;
                }
                img_close.Source = closeBtnHover;
            }));
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (closebtn == null)
                {
                    return;
                }

                img_close.Source = closebtn;
            }));
        }
    }
}
