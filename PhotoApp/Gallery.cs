namespace PhotoApp
{
    public class Gallery
    {
        public string GalleryId { get; set; }
        public string GalleryName { get; set; }

        public Gallery(string galleryId, string galleryName)
        {
            GalleryId = galleryId;
            GalleryName = galleryName;
        }
    }
}
