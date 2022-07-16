using DBDIconRepo.ViewModel;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    public class PerkSortByToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PerkSortBy mode)
            {
                if (parameter is string str)
                {
                    PerkSortBy compare = Enum.Parse<PerkSortBy>(str);
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
