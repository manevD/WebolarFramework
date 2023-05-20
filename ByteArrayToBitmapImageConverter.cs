using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Webolar.Framework;

public class ByteArrayToBitmapImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var img = new BitmapImage();
        if (value != null) img = ConvertByteArrayToBitMapImage(value as byte[]);
        return img;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }

    public BitmapImage ConvertByteArrayToBitMapImage(byte[] imageByteArray)
    {
        var img = new BitmapImage();
        using (var memStream = new MemoryStream(imageByteArray))
        {
            memStream.Position = 0;
            img.BeginInit();
            img.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = null;
            img.StreamSource = memStream;
            img.EndInit();
        }

        img.Freeze();
        return img;
    }
}