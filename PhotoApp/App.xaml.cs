using Xamarin.Forms;

namespace PhotoApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new UserSelectionPage())
            {
                BarBackgroundColor = Color.FromHex("#424e57"), BarTextColor = Color.White
            };
            MainPage = navigationPage;
        }

        protected override void OnStart()
        {
            Server.Client.Connect();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
