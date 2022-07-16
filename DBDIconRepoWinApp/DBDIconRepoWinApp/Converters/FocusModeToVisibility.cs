using DBDIconRepo.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class FocusModeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DetailFocusMode mode)
            {
                if (parameter is string compare)
                {
                    DetailFocusMode cmode = Enum.Parse<DetailFocusMode>(compare);
                    if (mode == cmode)
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
