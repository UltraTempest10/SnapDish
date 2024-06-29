using System;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace SnapDishApp
{
    public partial class DiaryListPage : ContentPage
    {
        public DiaryListPageViewModel ViewModel { get; set; }

        public DiaryListPage()
        {
            InitializeComponent();
            ViewModel = new DiaryListPageViewModel();
            BindingContext = ViewModel;
        }

        private async void OnDiaryItemTapped(object sender, EventArgs e)
        {
            if (sender is StackLayout stackLayout)
            {
                // 从 StackLayout 中获取 DiaryIdLabel 的值
                var diaryIdLabel = stackLayout.FindByName<Label>("DiaryIdLabel");
                if (diaryIdLabel != null && int.TryParse(diaryIdLabel.Text, out int diaryId))
                {
                    // 导航到详情页面
                    await Navigation.PushAsync(new DiaryDetailPage(diaryId));
                }
            }
        }
    }

    public class DiaryListPageViewModel
    {
        public ObservableCollection<DiaryEntry> DiaryEntries { get; set; }

        public DiaryListPageViewModel()
        {
            DiaryEntries = new ObservableCollection<DiaryEntry>();
            LoadFoodRecords();
        }

        private async void LoadFoodRecords()
        {

            var token = await SecureStorage.Default.GetAsync("auth_token");
            string url = HttpClientInstance.DiaryListUrl;
            var diaryEntries = await FetchDiaryEntriesAsync(url, token);
            DiaryEntries.Clear();
            if (diaryEntries != null)
            {
                foreach (var diaryEntry in diaryEntries)
                {
                    DiaryEntries.Add(diaryEntry);
                }
            }
        }

        private async Task<List<DiaryEntry>> FetchDiaryEntriesAsync(string url, string token)
        {
            try
            {
                HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await HttpClientInstance.HttpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var diaryEntries = JsonConvert.DeserializeObject<List<DiaryEntry>>(responseBody);
                return diaryEntries;
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
}
