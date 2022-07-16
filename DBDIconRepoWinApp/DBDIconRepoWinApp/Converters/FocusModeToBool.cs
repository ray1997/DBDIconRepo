using DBDIconRepo.ViewModel;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class FocusModeToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DetailFocusMode mode)
            {
                if (parameter is string str)
                {
                    DetailFocusMode compare = Enum.Parse<DetailFocusMode>(str);
                    if (mode == compare)
                        return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
