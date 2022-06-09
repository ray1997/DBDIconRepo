using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DBDIconRepo.Converters
{
    public class SortOptionToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Enum.Parse<Model.SortOptions>(value.ToString());
            var expect = Enum.Parse<Model.SortOptions>(parameter.ToString());
            if (input == expect)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
