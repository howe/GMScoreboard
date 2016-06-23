using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace GMScoreboardV6.Com
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:GMScoreboardV6.Com"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:GMScoreboardV6.Com;assembly=GMScoreboardV6.Com"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ImgTabItem/>
    ///
    /// </summary>
    public class ImgTabItem : TabItem
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource),
            typeof(ImgTabItem));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
            typeof(ImgTabItem));

        static ImgTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImgTabItem), new FrameworkPropertyMetadata(typeof(ImgTabItem)));
        }

        public ImageSource Icon
        {
            get { return (ImageSource)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            ImgTabItem imageTable = e.Source as ImgTabItem;

            if (imageTable == null) return;

            imageTable.Focus();
            base.OnSelected(e);
        }
    }

    public class TextBlockRun : TextBlock
    {
        public static readonly DependencyProperty RunListPropert =
            DependencyProperty.Register("RunList",typeof(Dictionary<string, string>), typeof(TextBlockRun),
            new PropertyMetadata(new PropertyChangedCallback(OrientationChangedCallback)));

        public static void OrientationChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                TextBlockRun control = d as TextBlockRun;
                if (control.RunList == null || control.RunList.Count <= 0)
                {
                    return;
                }

                control.Inlines.Clear();

                foreach (var item in control.RunList)
                {
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        continue;
                    }
                    control.Inlines.Add(" ");
                    control.Inlines.Add(new Run() { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.Value)), Text = item.Key });
                }

            }
            catch (Exception exception)
            {

            }
        }

        static TextBlockRun()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBlockRun), new FrameworkPropertyMetadata(typeof(TextBlockRun)));
        }

        public Dictionary<string,string> RunList
        {
            get { return (Dictionary<string, string>)this.GetValue(RunListPropert); }
            set { this.SetValue(RunListPropert, value); }
        }
   
    }
}