namespace SnapDishApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnRecordFoodClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecordingPage());
        }

        private async void OnViewDiaryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DiaryListPage());
        }

        private async void OnGetRecommendationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchingPage());
        }

        private async void OnViewDataClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StatisticsPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert("退出登录", "确定要退出登录吗？", "是", "否");
            if (confirmLogout)
            {
                // 清除本地存储的认证令牌
                SecureStorage.Default.Remove("auth_token");

                // 导航到登录页面
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
