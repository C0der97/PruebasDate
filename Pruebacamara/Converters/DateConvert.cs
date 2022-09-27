using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Pruebacamara.Converters
{
    public class DateConvert : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                string val = value.ToString();
                return string.Format("{0:dd-mm-yyyy}", val);
                //return AddSeparatorWriting(val);
            }

            return (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString();

            return val;
        }

        private string AddSeparatorWriting(string text)
        {

            const string separator = "/";

            text = text.Replace(separator, string.Empty);

            // 12
            const string patternTwoNumbers = @"^\d{2}";
            // 12/12/
            const string patternFourNumbers = @"^\d{2}/\d{2}";

            int separatorCount = Regex.Matches(text, separator).Count;


            if (Regex.IsMatch(text, patternTwoNumbers, RegexOptions.IgnoreCase) && separatorCount == 0)
            {
                text = text.Insert(2, separator);
            }

            separatorCount = Regex.Matches(text, separator).Count;

            if (Regex.IsMatch(text, patternFourNumbers, RegexOptions.IgnoreCase) && separatorCount == 1)
            {
                text = text.Insert(5, separator);
            }

            return text;
        }

    }
}
