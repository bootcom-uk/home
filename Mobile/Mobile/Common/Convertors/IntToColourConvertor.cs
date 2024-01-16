using System.Globalization;

namespace Mobile.Common.Convertors
{
    public class IntToColourConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;

            var colorCode = value as int?;

            return Color.FromInt(colorCode.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Color).ToInt();
        }
    }
}
