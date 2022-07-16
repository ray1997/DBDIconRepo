using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class NullableStringPropertyDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return "N/A";
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
