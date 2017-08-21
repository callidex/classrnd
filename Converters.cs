using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Callidex
{
    public class NegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object paramter, CultureInfo culture)
        {
            return Convert(value, targetType, paramter, culture);
        }
    }

    public class SortDirectionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            switch ((ListSortDirection) value)
            {
                case ListSortDirection.Ascending:
                    return "Ascending";
                case ListSortDirection.Descending:
                    return "Descending";
                default:
                    break;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string) value)
            {
                case "null":
                    return null;
                case "Ascending":
                    return ListSortDirection.Ascending;
                case "Descending":
                    return ListSortDirection.Descending;
                default:
                    break;
            }

            return null;
        }
    }

}
