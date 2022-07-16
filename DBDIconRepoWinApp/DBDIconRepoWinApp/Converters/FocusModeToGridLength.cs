using DBDIconRepo.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class FocusModeToGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DetailFocusMode dfm)
            {
                if (dfm == DetailFocusMode.Overview)
                    return GridLength.Auto;
                if (parameter is string set)
                {
                    DetailFocusMode compare = Enum.Parse<DetailFocusMode>(set);
                    if (compare == dfm)
                        return new GridLength(1, GridUnitType.Star);
                    return new GridLength(0, GridUnitType.Pixel);
                }
            }
            return GridLength.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
