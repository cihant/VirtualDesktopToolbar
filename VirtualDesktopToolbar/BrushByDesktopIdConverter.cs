using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VirtualDesktopToolbar
{
    internal class BrushByDesktopIdConverter : IValueConverter
    {
        private readonly SolidColorBrush _activeBrush = new SolidColorBrush(SystemColors.InactiveCaptionColor);
        private readonly SolidColorBrush _inActiveBrush = new SolidColorBrush(Colors.Black);

        public BrushByDesktopIdConverter(SolidColorBrush activeBrush, SolidColorBrush inActiveBrush)
        {
            _activeBrush = activeBrush;
            _inActiveBrush = inActiveBrush;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.Equals(parameter))
            {
                return _activeBrush;
            }
            return _inActiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}