using FFImageLoading.Forms.Platform;

namespace PhotoApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            CachedImageRenderer.Init();
            LoadApplication(new PhotoApp.App());
        }
    }
}
