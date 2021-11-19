using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NativeMedia;
using SkiaSharp;

namespace PhotoApp
{
    public static class Client
    {
        // public static async Task UploadImages(IEnumerable<IMediaFile> imagesToUpload)
        // {
        //     foreach (var image in imagesToUpload)
        //     {
        //         await Server.UploadImage(await image.OpenReadAsync(), image.NameWithoutExtension + "." + image.Extension);
        //         await Server.UploadThumbnail(await Task.Run(async () => CreateThumbnail(await image.OpenReadAsync())), image.NameWithoutExtension + "." + image.Extension);
        //     }
        // }

        public static async Task UploadImage(IMediaFile image)
        {
            await Server.UploadImage(await image.OpenReadAsync(), image.NameWithoutExtension + "." + image.Extension);
            await Server.UploadThumbnail(await Task.Run(async () => CreateThumbnail(await image.OpenReadAsync())),
                image.NameWithoutExtension + "." + image.Extension);
        }


        public static async Task DownloadImage(GalleryImage image)
        {
            await SaveToLocalFolderAsync(await Task.Run(() => Server.GetImageStream(image.OriginalPath)), image.Name);
        }


        private static async Task SaveToLocalFolderAsync(Stream dataStream, string fileName)
        {
            await MediaGallery.SaveAsync(MediaFileType.Image, dataStream, fileName);
        }


        private static Stream CreateThumbnail(Stream imageStream)
        {
            var codec = SKCodec.Create(imageStream);
            var bitmap = AutoOrient(SKBitmap.Decode(codec), codec.EncodedOrigin);
            var originalWidth = bitmap.Width;
            var originalHeight = bitmap.Height;
            int newWidth;
            int newHeight;

            if (originalHeight <= 1280 && originalWidth <= 1280)
            {
                newWidth = originalWidth;
                newHeight = originalHeight;
            }
            else
            {
                var ratioX = (float)1280 / originalWidth;
                var ratioY = (float)1280 / originalHeight;
                var ratio = Math.Min(ratioX, ratioY);
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);
            }

            var resized = bitmap.Resize(new SKImageInfo(newWidth,newHeight), SKFilterQuality.Medium);
            var encodedResize = resized.Encode(SKEncodedImageFormat.Jpeg,30);

            return encodedResize.AsStream();
        }

        private static SKBitmap AutoOrient(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            SKBitmap rotated;
            switch (origin)
            {
                case SKEncodedOrigin.BottomRight:
                    using (var surface = new SKCanvas(bitmap))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap.Copy(), 0, 0);
                    }
                    return bitmap;
                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                default:
                    return bitmap;
            }
        }
    }
}
