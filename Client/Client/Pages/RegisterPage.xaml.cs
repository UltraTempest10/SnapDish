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
            await DisplayAlert("����", "���������䡣", "ȷ��");
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
                await DisplayAlert("�ɹ�", "��֤���ѷ���", "ȷ��");
            }
            else
            {
                await DisplayAlert("����", "��֤�뷢��ʧ��", "ȷ��");
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("���Ӵ���", "�޷����ӵ������������������������Ӳ����ԡ�", "ȷ��");
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", "�������������" + ex.Message, "ȷ��");
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
            await DisplayAlert("ע��ʧ��", "������������벻һ��", "ȷ��");
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
                await DisplayAlert("ע��ɹ�", "���ѳɹ�ע��", "ȷ��");
                await Navigation.PopAsync(); // Navigate back to login page
            }
            else
            {
                await DisplayAlert("ע��ʧ��", "ע��ʧ�ܣ�������", "ȷ��");
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("���Ӵ���", "�޷����ӵ������������������������Ӳ����ԡ�", "ȷ��");
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", "�������������" + ex.Message, "ȷ��");
        }
    }
}