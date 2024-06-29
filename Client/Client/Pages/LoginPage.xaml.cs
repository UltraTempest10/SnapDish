using System.Net.Http;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SnapDishApp
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            var loginData = new
            {
                email,
                password
            };

            string json = JsonConvert.SerializeObject(loginData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("Sending login request to server...");
            Console.WriteLine(HttpClientInstance.UserLoginUrl);

            HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(HttpClientInstance.UserLoginUrl, content);


            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                // Store token securely
                await SecureStorage.SetAsync("auth_token", responseObject.token);

                // Navigate to main page
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await DisplayAlert("µ«¬º ß∞‹", "” œ‰ªÚ√‹¬Î¥ÌŒÛ", "»∑∂®");
            }
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        public class LoginResponse
        {
            public string token { get; set; }
        }
    }
}