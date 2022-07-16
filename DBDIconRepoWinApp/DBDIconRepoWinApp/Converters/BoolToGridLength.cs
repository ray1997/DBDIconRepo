using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class BoolToExpandedGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return b ? GridLength.Auto : new GridLength(1, GridUnitType.Star);
            }
            return new GridLength(1, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
