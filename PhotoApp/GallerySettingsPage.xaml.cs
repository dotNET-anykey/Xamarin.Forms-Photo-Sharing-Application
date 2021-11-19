using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GallerySettingsPage : ContentPage
    {
        public ObservableCollection<string> GalleryUsers;
        public ObservableCollection<Gallery> GalleriesData;

        public GallerySettingsPage(ObservableCollection<Gallery> galleriesData)
        {
            InitializeComponent();
            this.GalleriesData = galleriesData;
            Title = "Gallery info";
        }

        protected override async void OnAppearing()
        {
            await Task.Run(InitializeGalleryUsers);

            var backgroundImage = new CachedImage
            {
                Aspect = Aspect.AspectFill,
                Source = "background.jpg"
            };

            var generalAbsoluteLayout = new AbsoluteLayout();
            generalAbsoluteLayout.Children.Add(backgroundImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            var usersList = new ListView
            {
                SeparatorColor = Color.FromHex("#c8cdd0"),
                SelectionMode = ListViewSelectionMode.None,
                ItemsSource = GalleryUsers,
                ItemTemplate = new DataTemplate(() =>
                {
                    var userIcon = new CachedImage
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = 10,
                        Source = "userDark.png"
                    };

                    var userLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 16,
                        TextColor = Color.FromHex("#424e57"),
                        TextTransform = TextTransform.Uppercase
                    };
                    userLabel.SetBinding(Label.TextProperty, ".");

                    var removeUserButton = new Button
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        TextColor = Color.FromHex("#ff2d55"),
                        BackgroundColor = Color.White,
                        Text = "Remove"
                    };
                    removeUserButton.SetBinding(Button.CommandParameterProperty, ".");
                    removeUserButton.Clicked += RemoveUserButton_Clicked;

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = {userIcon, userLabel, removeUserButton}
                        }
                    };
                })
            };

            var galleryNameLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "Gallery name",
                TextColor = Color.White,
                FontSize = 16
            };

            var entry = new Entry
            {
                Margin = 0,
                TextColor = Color.Black,
                PlaceholderColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Keyboard = Keyboard.Default,
                Placeholder = Server.ActiveGalleryName
            };
            entry.Completed += Entry_Completed;

            var entryBoxView = new BoxView { Color = Color.FromHex("#424e57") };
            var entryAbsoluteLayout = new AbsoluteLayout();
            entryAbsoluteLayout.Children.Add(entryBoxView, new Rectangle(0.5, 0, 1, 40), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            entryAbsoluteLayout.Children.Add(galleryNameLabel, new Rectangle(0.5, 0, 1, 40), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            entryAbsoluteLayout.Children.Add(entry, new Rectangle(0.5, 1, 1, 40), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            var entryFrame = new Frame
            {
                HeightRequest = 80,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                BackgroundColor = Color.White,
                CornerRadius = 0,
                HasShadow = false,
                IsClippedToBounds = true,
                Content = entryAbsoluteLayout
            };

            var usersLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "Assigned users",
                TextColor = Color.White,
                FontSize = 16
            };

            var usersBoxView = new BoxView { Color = Color.FromHex("#424e57") };
            var usersAbsoluteLayout = new AbsoluteLayout();
            usersAbsoluteLayout.Children.Add(usersBoxView, new Rectangle(0.5, 0, 1, 40), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            usersAbsoluteLayout.Children.Add(usersLabel, new Rectangle(0, 0, 1, 40), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            usersAbsoluteLayout.Children.Add(usersList, new Rectangle(0.5, 1, 1, 223), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            var usersFrame = new Frame
            {
                HeightRequest = 263,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                BackgroundColor = Color.White,
                CornerRadius = 0,
                HasShadow = false,
                IsClippedToBounds = true,
                Content = usersAbsoluteLayout
            };

            var addUserButton = new Button
            {
                Padding = 1,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Text = "ADD USER",
                CornerRadius = 20,
                BackgroundColor = Color.White,
                TextColor = Color.FromHex("#424e57")
            };
            addUserButton.Clicked += AddUserButton_Clicked;

            var usersStackLayout = new StackLayout
            {
                Margin = 0,
                Padding = 0,
                Children = {usersFrame, addUserButton}
            };

            var deleteGalleryButton = new Button
            {
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#00FFFFFF"),
                CornerRadius = 20,
                BorderColor = Color.FromHex("#ff2d55"),
                BorderWidth = 1,
                TextColor = Color.FromHex("#ff2d55"),
                Text = "DELETE GALLERY"
            };
            deleteGalleryButton.Clicked += DeleteGalleryButton_Clicked;


            var generalStackLayout = new StackLayout
            {
                Spacing = 20,
                Margin = new Thickness(20,20,20,40),
                Orientation = StackOrientation.Vertical,
                Children = { entryFrame, usersStackLayout, deleteGalleryButton }
            };

            generalAbsoluteLayout.Children.Add(generalStackLayout, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            Content = generalAbsoluteLayout;

            base.OnAppearing();
        }

        private async void RemoveUserButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var user = button.CommandParameter.ToString();

            if (Server.ActiveUserId == user.Remove(0, 5))
            {
                var response = await DisplayAlert("Leave gallery", "Do you want to leave this gallery? \nYou will no longer have access to it.", "Yes", "No");
                if (response)
                {
                    await Task.Run(() => Server.RemoveUserFromGallery(user.Remove(0, 5)));
                    GalleryUsers.Remove(user);
                    GalleriesData.Remove(GalleriesData.SingleOrDefault(x => x.GalleryId == Server.ActiveGalleryId));
                    if (GalleryUsers.Count == 0)
                    {
                        await Task.Run(Server.DeleteGallery);
                    }
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                var response = await DisplayAlert("User remove", "Do you want to remove " + user + " from this gallery?", "Yes", "No");
                if (response)
                {
                    await Task.Run(() => Server.RemoveUserFromGallery(user.Remove(0, 5)));
                    GalleryUsers.Remove(user);
                }
            }
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            var result = ((Entry)sender).Text;
            var response = await DisplayAlert("Gallery rename", "Do you want to rename this gallery to " + result + "?", "Yes", "No");

            if (response)
            {
                Server.ActiveGalleryName = result;
                await Task.Run(() => Server.RenameGallery(result));
            }

        }

        private async void AddUserButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UsersListPage(GalleryUsers));
        }

        private async void DeleteGalleryButton_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("Gallery delete", "Are you sure you want to delete this gallery?", "Yes", "No");

            if (response)
            {
                GalleriesData.Remove(GalleriesData.SingleOrDefault(x => x.GalleryId == Server.ActiveGalleryId));
                await Task.Run(Server.DeleteGallery);
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                await Navigation.PopAsync();
            }
        }

        private async void InitializeGalleryUsers()
        {
            if (GalleryUsers == null)
            {
                GalleryUsers = new ObservableCollection<string>();
                foreach (var user in await Server.GetActiveGalleryAssignedUsersAsync())
                {
                    GalleryUsers.Add("User " + user);
                }
            }
        }
    }
}