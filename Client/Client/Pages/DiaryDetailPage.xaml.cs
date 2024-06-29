using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SnapDishApp;

public partial class DiaryDetailPage : ContentPage
{
    private int _diaryId;
    private DiaryDetail _diaryDetail;

    public DiaryDetailPage(int diaryId)
    {
        InitializeComponent();
        _diaryId = diaryId;
        LoadDiaryDetail();
    }

    private async void LoadDiaryDetail()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        string url = HttpClientInstance.DiaryDetailUrl + $"?DiaryId={_diaryId}";

        var diaryDetail = await FetchDiaryDetailAsync(url, token);

        if (diaryDetail != null)
        {
            _diaryDetail = diaryDetail;

            TitleLabel.Text = _diaryDetail.Title;
            LocationLabel.Text = _diaryDetail.Location;
            CreateTimeLabel.Text = _diaryDetail.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            ContentLabel.Text = _diaryDetail.Content;
            RatingLabel.Text = $"评分: {_diaryDetail.Rating}";

            if (!string.IsNullOrEmpty(diaryDetail.Image))
            {
                ImageView.Source = diaryDetail.Image;
            }
            else
            {
                ImageView.IsVisible = false;
            }
        }
        else
        {
            await DisplayAlert("错误", "无法获取日记详情", "确认");
        }
    }

    private async Task<DiaryDetail> FetchDiaryDetailAsync(string url, string token)
    {
        try
        {
            HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClientInstance.HttpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var diaryDetail = JsonConvert.DeserializeObject<DiaryDetail>(responseBody);
            return diaryDetail;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"General error: {e.Message}");
            return null;
        }
    }
}

public class DiaryDetail
{
    public int DiaryId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string Content { get; set; }
    public string? Image { get; set; }
    public int Rating { get; set; }
    public DateTime CreateTime { get; set; }
}