using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    internal class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool input)
            {
                return input ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility input)
            {
                return input == Visibility.Visible ? true : false;
            }
            return false;
        }
    }
}
