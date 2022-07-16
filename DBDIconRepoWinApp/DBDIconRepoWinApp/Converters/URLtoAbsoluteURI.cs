using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace DBDIconRepo.Converters
{
    public class URLtoAbsoluteURI : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string url)
                return new BitmapImage(new Uri(url, UriKind.Absolute));
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
