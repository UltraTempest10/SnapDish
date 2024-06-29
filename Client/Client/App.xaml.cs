namespace SnapDishApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ContentPage()); // Temporary placeholder page

            // Simulate token check
            bool hasValidToken = CheckForValidToken().Result;

            if (hasValidToken)
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        private async Task<bool> CheckForValidToken()
        {
            Console.WriteLine("Checking for valid token");
            var token = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_token")).Result;
            Console.WriteLine("Check complete. Token: " + token);
            return !string.IsNullOrEmpty(token);
        }
    }

}

