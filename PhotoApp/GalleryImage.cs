using System.IO;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace PhotoApp
{
    public class GalleryImage
    {
        public string Name { get; }
        public string OriginalPath { get; }
        public string ThumbnailPath { get; }
        public ImageSource Source { get; }

        public GalleryImage(string imageName)
        {
            Name = imageName;
            OriginalPath = Server.GalleriesPath + Server.ActiveGalleryId + "/Images/" + imageName;
            ThumbnailPath = Server.GalleriesPath + Server.ActiveGalleryId + "/Thumbnails/" + imageName;
            var imageInBytes = Server.GetImageInBytes(ThumbnailPath);
            Source = new CustomStreamImageSource
            {
                Key = imageName,
                Stream = async token => new MemoryStream(imageInBytes)
            };
        }

        public class CustomStreamImageSource : StreamImageSource
        {
            public string Key { get; set; }
        }

        public class CustomCacheKeyFactory : ICacheKeyFactory
        {
            public string GetKey(ImageSource imageSource, object bindingContext)
            {
                var keySource = imageSource as CustomStreamImageSource;

                return keySource?.Key;
            }
        }

    }
}
