using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using NativeMedia;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryDetailPage : ContentPage
    {
        private List<GalleryImage> _selectedImages = new List<GalleryImage>();
        public ObservableCollection<Gallery> GalleriesData;

        public GalleryDetailPage(ObservableCollection<Gallery> galleriesData)
        {
            InitializeComponent();
            this.GalleriesData = galleriesData;
            BackgroundColor = Color.FromHex("#2c343a");
            Title = Server.ActiveGalleryName;
        }


        protected override async void OnAppearing()
        {
            var retrievedImages = await Task.Run(Server.GetImagesNames);

            if (retrievedImages.Count > 0)
            {
                var imagesCollection = new ObservableCollection<GalleryImage>();

                var collectionView = new CollectionView
                {
                    ItemsSource = imagesCollection,
                    SelectionMode = SelectionMode.Multiple,
                    //ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical),
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var cachedImage = new CachedImage
                        {
                            Aspect = Aspect.AspectFill,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            DownsampleToViewSize = true,
                            FadeAnimationEnabled = false,
                            CacheKeyFactory = new GalleryImage.CustomCacheKeyFactory() //new GalleryImage.CustomCacheKeyFactory()
                        };
                        cachedImage.SetBinding(CachedImage.SourceProperty, "Source", BindingMode.OneTime);

                        var stackLayout = new StackLayout
                        {
                            Padding = 2,
                            Children = { cachedImage }
                        };

                        return stackLayout;
                    })
                };
                collectionView.SelectionChanged += CollectionView_SelectionChanged;
                Content = collectionView;

                foreach (var image in retrievedImages)
                {
                    await Task.Run(() => imagesCollection.Add(new GalleryImage(image)));
                }
            }
            else
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Gallery has no photos", FontSize = 20, HorizontalOptions = LayoutOptions.Center}
                    }
                };
            }


            base.OnAppearing();
        }


        protected override async void OnDisappearing()
        {
            //await ImageService.Instance.InvalidateCacheAsync(CacheType.Memory);
            _selectedImages.Clear();
            base.OnDisappearing();
            

        }


        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                _selectedImages.Clear();
                foreach (var selectedImage in e.CurrentSelection)
                {
                    _selectedImages.Add(selectedImage as GalleryImage);
                }

                if (_selectedImages != null)
                {
                    ToolbarItem.IconImageSource = "download.png";
                    ToolbarItem2.IconImageSource = "cancel.png";
                }
            }
            else
            {
                _selectedImages.Clear();
                ToolbarItem.IconImageSource = "upload.png";
                ToolbarItem2.IconImageSource = "list.png";
            }
        }


        private async void ToolbarItem_OnClicked(object sender, EventArgs e)
        {
            if (_selectedImages.Count == 0)
            {
                var pickedImages = await MediaGallery.PickAsync(9999, MediaFileType.Image);
                
                if (pickedImages.Files != null)
                {
                    foreach (var image in pickedImages.Files)
                    {
                        await Client.UploadImage(image);
                    }
                }
            }
            else if (_selectedImages.Count > 0)
            {
                await DisplayAlert("Download", "Download started", "OK");
                foreach (var selectedImage in _selectedImages)
                {
                    await Client.DownloadImage(selectedImage);
                }
            }
        }

        private async void ToolbarItem2_OnClicked(object sender, EventArgs e)
        {
            if (_selectedImages.Count == 0)
            {
                await Navigation.PushAsync(new GallerySettingsPage(GalleriesData), true);
            }
            else if (_selectedImages.Count > 0)
            {

            }
        }
    }
}
