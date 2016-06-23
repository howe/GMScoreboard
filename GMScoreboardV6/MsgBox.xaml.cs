using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GMScoreboardV6
{
    /// <summary>
    /// MsgBox.xaml 的交互逻辑
    /// </summary>
    public partial class MsgBox : Window
    {
        public MsgBox()
        {
            InitializeComponent();
        }

        public static void Show(ImageSource icon, string title, string content,
            string btnText)
        {
            var msgbox = new MsgBox();
            msgbox.icon.Source = icon;
            msgbox.title.Text = title;
            msgbox.Title = title;
            msgbox.content.Text = content;
            msgbox.btnText.Content = btnText;
            msgbox.Left = App.Current.MainWindow.Left + (App.Current.MainWindow.Width - msgbox.Width) / 2;
            msgbox.Top = App.Current.MainWindow.Top + (App.Current.MainWindow.Height - msgbox.Height) / 2;
            msgbox.ShowDialog();
        }

        private void btnText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}