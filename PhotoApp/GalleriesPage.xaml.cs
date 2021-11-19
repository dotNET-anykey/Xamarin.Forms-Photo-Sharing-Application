using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleriesPage : ContentPage
    {
        public ObservableCollection<Gallery> GalleriesData;
        private List<string> userGalleriesIDs;

        public GalleriesPage()
        {
            InitializeComponent();

        }

        protected override async void OnAppearing()
        {
            var generalAbsoluteLayout = new AbsoluteLayout();

            var backgroundImage = new CachedImage
            {
                Aspect = Aspect.AspectFill,
                Source = "background.jpg"
            };
            generalAbsoluteLayout.Children.Add(backgroundImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            Content = generalAbsoluteLayout;

            if (GalleriesData == null)
            {
                GalleriesData = new ObservableCollection<Gallery>();
                userGalleriesIDs = await Server.GetActiveUserAssignedGalleriesAsync();
            }

            if (userGalleriesIDs.Count > 0 || GalleriesData.Count > 0)
            {
                var collectionView = new CollectionView
                {
                    ItemsSource = GalleriesData,
                    Margin = 10,
                    SelectionMode = SelectionMode.Single,
                    ItemSizingStrategy = ItemSizingStrategy.MeasureAllItems,
                    ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical),
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var image = new CachedImage
                        {
                            Source = "gallery.png"
                        };
                        var boxView = new BoxView { Color = Color.FromHex("#424e57") };
                        var galleryName = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = Color.White,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16
                        };
                        galleryName.SetBinding(Label.TextProperty, "GalleryName");
                        var galleryId = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.End,
                            TextColor = Color.FromHex("#c8cdd0")
                        };
                        galleryId.SetBinding(Label.TextProperty, new Binding { Path = "GalleryId", StringFormat = "ID: {0}" });

                        var absoluteLayout = new AbsoluteLayout();
                        absoluteLayout.Children.Add(image, new Rectangle(0.5, 0, 0.7, 0.7), AbsoluteLayoutFlags.All);
                        absoluteLayout.Children.Add(boxView, new Rectangle(0.5, 1, 1.5, 0.3), AbsoluteLayoutFlags.All);
                        absoluteLayout.Children.Add(galleryName, new Rectangle(0.5, 1, 1, 0.30), AbsoluteLayoutFlags.All);
                        absoluteLayout.Children.Add(galleryId, new Rectangle(0.98, 0.675, 0.5, 0.15), AbsoluteLayoutFlags.All);

                        var itemFrame = new Frame
                        {
                            WidthRequest = Height,
                            Padding = 0,
                            BackgroundColor = Color.White,
                            CornerRadius = 0,
                            HasShadow = false,
                            IsClippedToBounds = true,
                            Content = absoluteLayout
                        };

                        var stackLayout = new StackLayout
                        {
                            Padding = 10,
                            Children = {itemFrame}
                        };

                        return stackLayout;
                    })
                };
                collectionView.SelectionChanged += CollectionView_SelectionChanged;

                generalAbsoluteLayout.Children.Add(collectionView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

                if (GalleriesData.Count == 0)
                {
                    userGalleriesIDs.Reverse();
                    foreach (var galleryId in userGalleriesIDs)
                    {
                        GalleriesData.Add(await Server.GetGalleryDataAsync(galleryId));
                    }
                }
            }
            else
            {
                var  stackLayout = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "You have no galleries", TextColor = Color.FromHex("#424e57"), FontSize = 20, HorizontalOptions = LayoutOptions.Center}
                    }
                };
                generalAbsoluteLayout.Children.Add(stackLayout, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            }

            base.OnAppearing();
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((CollectionView)sender).SelectedItem == null)
                return;

            var gallery = ((CollectionView)sender).SelectedItem as Gallery;

            Server.ActiveGalleryId = gallery.GalleryId;
            Server.ActiveGalleryName = gallery.GalleryName;

            await Navigation.PushAsync(new GalleryDetailPage(GalleriesData), true);

            ((CollectionView)sender).SelectedItem = null;
        }


        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var galleryName = await DisplayPromptAsync("New Gallery", "Please enter a name for a new Gallery");
            if (galleryName != null)
            {
                GalleriesData.Add(await Task.Run(() => Server.CreateNewGallery(galleryName)));

                var galleriesCount = GalleriesData.Count;
                if (galleriesCount > 0)
                {
                    GalleriesData.Move(galleriesCount - 1, 0);
                }
                if (galleriesCount == 1)
                {
                    OnAppearing();
                }
                
            }
        }
    }
}