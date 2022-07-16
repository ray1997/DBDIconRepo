using DBDIconRepo.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace DBDIconRepo.Converters
{
    /// <summary>
    /// Return a 1* grid state on fail condition or None state
    /// otherwise make it auto
    /// This is for the case when normal mode an install and detail button is split equally left and right
    /// [ Install ] | [Detail]
    /// while the pack is download or install the detail button shrink and display a progress
    /// [ ████████░░░░░ ]  [!]
    /// </summary>
    public class PackStateToGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PackState state)
            {
                if (state == PackState.None)
                    return new GridLength(1, GridUnitType.Star);
                else
                    return GridLength.Auto;
            }
            return new GridLength(1, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
