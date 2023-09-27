using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace CSWall;

public static class BitmapUtils
{
    public static Bitmap GetBitmapByImageSource(BitmapSource source, bool isPng = false)
    {
        using var ms = new MemoryStream();
        BitmapEncoder encoder = isPng
          ? new PngBitmapEncoder()
          : new BmpBitmapEncoder()
          ;
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(ms);
        var bp = new Bitmap(ms);
        return new Bitmap(bp);
    }
    //BitmapImage to Bitmap
    public static Bitmap GetBitmapByBitmapImage(BitmapImage bitmapImage, bool isPng = false)
    {
        using var ms = new MemoryStream();
        BitmapEncoder encoder = isPng
            ? new PngBitmapEncoder()
            : new BmpBitmapEncoder()
            ;
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        encoder.Save(ms);
        var bp = new Bitmap(ms);
        return new Bitmap(bp);
    }
    // Bitmap  to BitmapImage
    public static BitmapImage GetBitmapImageBybitmap(Bitmap bitmap)
    {
        var bitmapImage = new BitmapImage();
        try
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
        catch (Exception)
        {
            return null;
        }
    }
}