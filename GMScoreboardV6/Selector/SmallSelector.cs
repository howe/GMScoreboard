using GMScoreboardV6.Models;
using System.Windows;
using System.Windows.Controls;

namespace GMScoreboardV6.Selector
{
    public class SmallSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }

        public DataTemplate MyAwardTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var award = item as AwardConfig;

            if (award != null && award.myAward
                && award.status == 0)
            {
                return MyAwardTemplate;
            }
            else
            {
                return NormalTemplate;
            }
        }
    }
}