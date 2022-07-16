using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class SortOptionToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var input = Enum.Parse<Model.SortOptions>(value.ToString());
            var expect = Enum.Parse<Model.SortOptions>(parameter.ToString());
            if (input == expect)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
