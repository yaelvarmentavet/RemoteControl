using System;
using System.Globalization;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullable = value as uint?;
            var result = string.Empty;

            if (nullable.HasValue)
            {
                if (nullable.Value == DataModel.UERROR)
                    result = "N/A";
                else
                    result = nullable.Value.ToString();

            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            int intValue;
            int? result = null;

            if (int.TryParse(stringValue, out intValue))
            {
                result = new Nullable<int>(intValue);
            }

            return result;
        }
    }


}