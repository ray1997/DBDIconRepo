using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class ShowSpecificTextIfNullConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return parameter.ToString();
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
