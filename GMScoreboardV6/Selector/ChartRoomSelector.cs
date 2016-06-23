using GMScoreboardV6.Models;
using System.Windows;
using System.Windows.Controls;

namespace GMScoreboardV6.Selector
{
    public class ChartRoomSelector : DataTemplateSelector
    {
        public DataTemplate WebBrowerDataTemplate { get; set; }

        public DataTemplate ApplicationDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var menu = item as MenuConfig;

            if (menu != null && menu.type == 1)
            {
                return WebBrowerDataTemplate;
            }
            else
            {
                return ApplicationDataTemplate;
            }
        }
    }
}
