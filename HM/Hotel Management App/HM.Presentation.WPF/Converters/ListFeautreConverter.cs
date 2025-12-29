using System.Globalization;
using System.Text;
using System.Windows.Data;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Presentation.WPF.Converters;

public class ListFeautreConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is List<Feature> feautre)
        {
            var isFirst = true;
            StringBuilder sb = new();
            foreach (var feautreItem in feautre)
            {
                if (isFirst)
                {
                    sb.Append(feautreItem.ToString());
                    isFirst = false;
                    continue;
                }

                sb.Append($", {feautreItem.ToString()}");
            }

            return sb.ToString();
        }

        throw new ArgumentException("Value is not a List of Feautre");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}