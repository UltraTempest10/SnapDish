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
    }
}
