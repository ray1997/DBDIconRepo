using DBDIconRepo.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class PackStateToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PackState state)
            {
                if (parameter is string compare)
                {
                    PackState cs = Enum.Parse<PackState>(compare);
                    if (state == cs)
                        return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
