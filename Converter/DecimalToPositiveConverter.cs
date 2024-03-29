﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Webolar.Framework;

public class DecimalToPositiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal)
            return 0 <= (decimal)value;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool)
        {
            if ((bool)value)
                return 1;
            return -1;
        }

        return 0;
    }
}