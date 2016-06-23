using GMScoreboardV6.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GMScoreboardV6.Com
{
    /// <summary>
    /// ChartRoom.xaml 的交互逻辑
    /// </summary>
    public partial class ChartRoom : UserControl
    {
        public ChartRoom()
        {
            InitializeComponent();
        }

        TextDecoration txtDecordation = new TextDecoration();
        Brush noSelectBrush= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
        Brush selectBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF962E"));


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = null;

            if (sender == btnMessage)
            {
                textBlock = txtMessage;
                txtOnLinePeoper.Foreground = noSelectBrush;
                txtOnLinePeoper.TextDecorations.Clear();
                borderOnLinePeoper.Visibility = Visibility.Hidden;
                borderMessage.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock = txtOnLinePeoper;
                txtMessage.Foreground = noSelectBrush;
                txtMessage.TextDecorations.Clear();
                borderOnLinePeoper.Visibility = Visibility.Visible;
                borderMessage.Visibility = Visibility.Hidden;
                GetOnlinePlayers();
            }

            if (textBlock == null)
            {
                return;
            }

            textBlock.Foreground = selectBrush;
            textBlock.TextDecorations.Clear();
            textBlock.TextDecorations.Add(txtDecordation);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Pen selectPen = new Pen();
            selectPen.Brush = selectBrush;

            txtDecordation.PenOffset = 5;
            txtDecordation.PenOffsetUnit = TextDecorationUnit.Pixel;
            txtDecordation.PenThicknessUnit = TextDecorationUnit.Pixel;
            txtDecordation.Pen = selectPen;
        }

        void GetOnlinePlayers()
        {
            var onLinePlayers = ChartRoomMng.GetOnlinePlayers();

            this.Dispatcher.Invoke(new Action(() =>
            {
                txtOnLinePeoper.Text = string.Format("在线人数({0})人）", onLinePlayers.data.totalPlayer);
                MyOnline.ItemsSource = onLinePlayers.data.players;
            }));
        }
    }
}
