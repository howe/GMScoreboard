
using GMScoreboardV6.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GMScoreboardV6
{
    /// <summary>
    /// ClientTipForm.xaml 的交互逻辑
    /// </summary>
    public partial class ClientTipForm : Window
    {
        public ClientTipForm()
        {
            InitializeComponent();
        }

        private ClientTip tip;

        public ClientTipForm(ClientTip tip) : this()
        {
            this.tip = tip;
            this.lblHead.Content = tip.headText;
            this.linkText.Text = tip.linkText;
            this.lblTitle.Content = tip.title;
            this.txtContent.Text = "    " + tip.content;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;

            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height;

            DoubleAnimation animtion = new DoubleAnimation()
            {
                From = SystemParameters.WorkArea.Height,
                To = SystemParameters.WorkArea.Height - this.Height,
                Duration = TimeSpan.FromSeconds(5),
                FillBehavior = FillBehavior.HoldEnd,
                AccelerationRatio = .5,

            };

            DoubleAnimation animtion2 = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(5),
                FillBehavior = FillBehavior.HoldEnd,
                AccelerationRatio = .5,

            };

            animtion.Completed += Animtion_Completed;
            this.BeginAnimation(Window.TopProperty, animtion);
            this.BeginAnimation(Window.OpacityProperty, animtion2);
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        private void Animtion_Completed(object sender, EventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            DoubleAnimation closeAnimtion = new DoubleAnimation()
            {
                From = SystemParameters.WorkArea.Height - this.Height,
                To = SystemParameters.WorkArea.Height + 50,
                Duration = TimeSpan.FromSeconds(5),
            };

            closeAnimtion.Completed += closeAnimtion_Completed;
            this.BeginAnimation(Window.TopProperty, closeAnimtion);
           
        }

        private void closeAnimtion_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gotoDetail(object sender, RoutedEventArgs e)
        {
            if (tip.browser != null && !tip.browser.Trim().Equals(""))
            {
                System.Diagnostics.Process.Start(tip.browser, tip.link);
            }
            else if (!string.IsNullOrEmpty(tip.link))
            {
                System.Diagnostics.Process.Start("explorer.exe", tip.link);
            }
            this.Close();
        }

        private void txtContent_MouseEnter(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }

        private void txtContent_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
        }
    }
}
