using System;
using Windows.UI.Xaml.Data;

namespace Discord.Converters
{
    class InverseStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null || ((string)value) == string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
