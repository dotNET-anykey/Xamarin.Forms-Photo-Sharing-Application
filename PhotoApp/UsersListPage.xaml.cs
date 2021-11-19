using FFImageLoading.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UsersListPage : ContentPage
    {
        private ObservableCollection<string> _galleryUsers;

        public UsersListPage (ObservableCollection<string> galleryUsers)
		{
			InitializeComponent ();
            this._galleryUsers = galleryUsers;
            Title = "Add user";
        }

        protected override async void OnAppearing()
        {
            var backgroundImage = new CachedImage
            {
                Aspect = Aspect.AspectFill,
                Source = "background.jpg"
            };

            var generalAbsoluteLayout = new AbsoluteLayout();
            generalAbsoluteLayout.Children.Add(backgroundImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            var listView = new ListView
            {
                RowHeight = 50,
                ItemsSource = await Task.Run(Server.GetNotAssignedUsers),
                ItemTemplate = new DataTemplate(() =>
                {
                    var label = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 25,
                    };
                    label.SetBinding(Label.TextProperty,".");

                    var viewCell = new ViewCell
                    {
                        View = label
                    };

                    return viewCell;
                })
            };
            listView.ItemSelected += ListView_ItemSelected;

            var generalFrame = new Frame
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 300,
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.1),
                CornerRadius = 0,
                HasShadow = false,
                Content = listView
            };

            generalAbsoluteLayout.Children.Add(generalFrame, new Rectangle(0, 0, 1, 0.5), AbsoluteLayoutFlags.All);

            Content = generalAbsoluteLayout;

            base.OnAppearing();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var user = e.SelectedItem as string;

            var response = await DisplayAlert("User add", "Do you want to add " + user + " to this gallery?", "Yes", "No");

            if (response)
            {
                await Task.Run(() => Server.AssignUserToGallery(user.Remove(0, 5)));
                _galleryUsers.Add(user);
                await Navigation.PopAsync(true);
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}