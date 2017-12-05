using Discord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Discord.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 116, 127, 141));
            }

            switch ((Status)value)
            {
                case Status.Active:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 67, 181, 129));
                case Status.Away:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 250, 166, 26));
                case Status.Offline:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 116, 127, 141));
                default:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 116, 127, 141));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
