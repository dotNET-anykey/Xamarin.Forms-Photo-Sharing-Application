using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSelectionPage : ContentPage
    {
        private ObservableCollection<string> _allUsers;
        private string _selectedUser ="";
        private const string GreyColor = "#c8cdd0";
        private const string Version = "HDshare v2.3";
        private Button _logInButton;

        public UserSelectionPage()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            _allUsers = new ObservableCollection<string>(await Task.Run(Server.GetAllUsers));

            var picker = new Picker
            {
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemsSource = _allUsers,
                TextColor = Color.FromHex(GreyColor),
                TitleColor = Color.FromHex(GreyColor),
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                HorizontalTextAlignment = TextAlignment.Center,
                Title = "SELECT USER"
            };
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            var pickerImage = new CachedImage
            {
                Margin = 10,
                Source = "group.png"
            };

            var pickerBoxView = new BoxView { Color = Color.White };
            var pickerBoxView2 = new BoxView { Color = Color.FromHex(GreyColor) };

            var pickerAbsoluteLayout = new AbsoluteLayout();
            pickerAbsoluteLayout.Children.Add(picker, new Rectangle(0, 0.5, 0.82, 1), AbsoluteLayoutFlags.All);
            pickerAbsoluteLayout.Children.Add(pickerBoxView, new Rectangle(0.5, 0.851, 1.5, 0.05), AbsoluteLayoutFlags.All);
            pickerAbsoluteLayout.Children.Add(pickerBoxView2, new Rectangle(1, 0.5, 0.18, 1.5), AbsoluteLayoutFlags.All);
            pickerAbsoluteLayout.Children.Add(pickerImage, new Rectangle(1, 0.5, 0.18, 1), AbsoluteLayoutFlags.All);

            var pickerFrame = new Frame
            {
                Margin = 0,
                Padding = 0,
                CornerRadius = 0,
                HeightRequest = 50,
                BackgroundColor = Color.White,
                HasShadow = false,
                IsClippedToBounds = true,
                Content = pickerAbsoluteLayout
            };

            _logInButton = new Button
            {
                Padding = 0,
                BackgroundColor = Color.White,
                TextColor = Color.FromHex(GreyColor),
                Text = "Log in",
            };
            _logInButton.Clicked += LogInButton_Clicked;

            var loginImage = new CachedImage
            {
                Margin = 10,
                Source = "user.png"
            };

            var loginBoxView = new BoxView { Color = Color.FromHex(GreyColor) };

            var loginAbsoluteLayout = new AbsoluteLayout();
            loginAbsoluteLayout.Children.Add(_logInButton, new Rectangle(0, 0.5, 0.82, 1), AbsoluteLayoutFlags.All);
            loginAbsoluteLayout.Children.Add(loginBoxView, new Rectangle(1, 0.5, 0.18, 1.5), AbsoluteLayoutFlags.All);
            loginAbsoluteLayout.Children.Add(loginImage, new Rectangle(1, 0.5, 0.18, 1), AbsoluteLayoutFlags.All);

            var loginFrame = new Frame
            {
                Margin = 0,
                Padding = 0,
                CornerRadius = 0,
                HeightRequest = 50,
                BackgroundColor = Color.White,
                HasShadow = false,
                IsClippedToBounds = true,
                Content = loginAbsoluteLayout
            };

            var createUserButton = new Button
            {
                Padding = 0,
                BackgroundColor = Color.White,
                TextColor = Color.FromHex(GreyColor),
                Text = "Create new user"
            };
            createUserButton.Clicked += CreateNewUserButton_Clicked;

            var createUserImage = new CachedImage
            {
                Margin = 10,
                Source = "user1.png"
            };

            var createUserBoxView = new BoxView { Color = Color.FromHex(GreyColor) };

            var createUserAbsoluteLayout = new AbsoluteLayout();
            createUserAbsoluteLayout.Children.Add(createUserButton, new Rectangle(0, 0.5, 0.82, 1), AbsoluteLayoutFlags.All);
            createUserAbsoluteLayout.Children.Add(createUserBoxView, new Rectangle(1, 0.5, 0.18, 1.5), AbsoluteLayoutFlags.All);
            createUserAbsoluteLayout.Children.Add(createUserImage, new Rectangle(1, 0.5, 0.18, 1), AbsoluteLayoutFlags.All);

            var createUserFrame = new Frame
            {
                Margin = 0,
                Padding = 0,
                CornerRadius = 0,
                HeightRequest = 50,
                BackgroundColor = Color.White,
                HasShadow = false,
                IsClippedToBounds = true,
                Content = createUserAbsoluteLayout
            };

            var stackLayout = new StackLayout
            {
                Padding = 0,
                Spacing = 20,
                Children = { pickerFrame, loginFrame }
            };

            var logo = new CachedImage
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 200,
                WidthRequest = 200,
                Source = "logo6.png"
            };

            var generalFrame = new Frame
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 300,
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.1),
                CornerRadius = 0,
                HasShadow = false,
                Content = stackLayout
            };

            var createUserFrame2 = new Frame
            {
                Margin = 0,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 300,
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.1),
                CornerRadius = 0,
                HasShadow = false,
                Content = createUserFrame
            };

            var version = new Label
            {
                Padding = 10,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                Text = Version,
                TextColor = Color.White
            };

            var generalStackLayout = new StackLayout
            {
                Spacing = 20,
                Padding = new Thickness(0, 100, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { logo, generalFrame, createUserFrame2, version }
            };

            var backgroundImage = new CachedImage
            {
                Aspect = Aspect.AspectFill,
                Source = "background.jpg"
            };

            var shadow = new CachedImage
            {
                Margin = 0,
                VerticalOptions = LayoutOptions.Start,
                Source = "shadow.png"
            };

            var generalAbsoluteLayout = new AbsoluteLayout();
            generalAbsoluteLayout.Children.Add(backgroundImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            generalAbsoluteLayout.Children.Add(shadow, new Rectangle(0.5, 0, 1,0.2), AbsoluteLayoutFlags.All);
            generalAbsoluteLayout.Children.Add(generalStackLayout, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            Content = generalAbsoluteLayout;

            base.OnAppearing();
        }

        private async void CreateNewUserButton_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("New User", "Do you want to create new User?", "Yes", "No");

            if (response)
            {
                _allUsers.Add(await Task.Run(Server.CreateNewUser));
                await DisplayAlert("New User", "Now you can select newly created user.", "OK");
            }
        }

        private async void LogInButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedUser != "")
            {
                Server.ActiveUserId = _selectedUser.Remove(0,5);
                _selectedUser = "";
                await Navigation.PushAsync(new GalleriesPage(), true);
            }
            else
            {
                await DisplayAlert("Please select user", "You need to select user!", "OK");
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedUser = ((Picker) sender).Items[((Picker) sender).SelectedIndex];
            _logInButton.TextColor = Color.Accent;
        }
    }
}