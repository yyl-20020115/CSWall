using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace CSWall;

public static class BitmapUtils
{
    public static Bitmap GetBitmapByImageSource(BitmapSource source, bool isPng = false)
    {
        using var stream = new MemoryStream();
        BitmapEncoder encoder = isPng
          ? new PngBitmapEncoder()
          : new BmpBitmapEncoder()
          ;
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(stream);
        using var bp = new Bitmap(stream);
        return new Bitmap(bp);
    }
    //BitmapImage to Bitmap
    public static Bitmap GetBitmapByBitmapImage(BitmapImage bitmapImage, bool isPng = false)
    {
        using var stream = new MemoryStream();
        BitmapEncoder encoder = isPng
            ? new PngBitmapEncoder()
            : new BmpBitmapEncoder()
            ;
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        encoder.Save(stream);
        using var bp = new Bitmap(stream);
        return new Bitmap(bp);
    }
    // Bitmap  to BitmapImage
    public static BitmapImage GetBitmapImageBybitmap(Bitmap bitmap)
    {
        var image = new BitmapImage();
        try
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception)
        {
            return null;
        }
    }
}