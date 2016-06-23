using GMScoreboardV6.Utils;
using System;
using System.Windows;
using static GMScoreboardV6.MainWindow;

namespace GMScoreboardV6
{
    /// <summary>
    /// PersonalCenter.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalCenter : Window
    {
        private string _url = string.Empty;

        public PersonalCenter()
        {
            InitializeComponent();

            web_personCenter.ObjectForScripting = new ScriptEvent(this);
            WinApi.SuppressScriptErrors(web_personCenter, true);
        }

        public PersonalCenter(string url) : this()
        {
            _url = url;
            web_personCenter.Source = new Uri(url);
        }

        public PersonalCenter(string title, string url, int height, int width) : this(url)
        {
            this.Title = title;
            this.Width = width;
            this.Height = height;
        }

        public void Refsah()
        {
            web_personCenter.Source = new Uri(_url);
        }

        public void Refsah(string url)
        {
            _url = url;
            web_personCenter.Source = new Uri(_url);
        }
    }
}
