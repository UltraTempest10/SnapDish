namespace Client.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void OnLoginClicked(object sender, EventArgs e)
    {
        // ��¼
		

        await Navigation.PushAsync(new MainPage());
    }
}