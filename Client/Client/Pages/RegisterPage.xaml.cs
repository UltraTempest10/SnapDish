using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace SnapDishApp;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private async void OnGetVarificationCodeButtonClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("错误", "请输入邮箱。", "确认");
            return;
        }

        // Construct the URL with the query parameter
        string requestUri = HttpClientInstance.UserSendUrl + $"?email={email}";

        // The body can be empty or contain some placeholder data
        StringContent content = new StringContent("", Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("成功", "验证码已发送", "确认");
            }
            else
            {
                await DisplayAlert("错误", "验证码发送失败", "确认");
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("连接错误", "无法连接到服务器。请检查您的网络连接并重试。", "确认");
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", "发生了意外错误：" + ex.Message, "确认");
        }
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;
        string verificationCode = VarificationCodeEntry.Text;

        if (password != confirmPassword)
        {
            await DisplayAlert("注册失败", "两次输入的密码不一致", "确认");
            return;
        }

        var registerData = new
        {
            email,
            password,
        };

        string requestUri = HttpClientInstance.UserRegisterUrl + $"?code={verificationCode}";

        string json = JsonConvert.SerializeObject(registerData);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("注册成功", "您已成功注册", "确认");
                await Navigation.PopAsync(); // Navigate back to login page
            }
            else
            {
                await DisplayAlert("注册失败", "注册失败，请重试", "确认");
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("连接错误", "无法连接到服务器。请检查您的网络连接并重试。", "确认");
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", "发生了意外错误：" + ex.Message, "确认");
        }
    }
}