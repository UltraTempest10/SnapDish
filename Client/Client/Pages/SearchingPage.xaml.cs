using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace SnapDishApp
{
    public partial class SearchingPage : ContentPage
    {
        public SearchingPageViewModel ViewModel { get; set; }

        public SearchingPage()
        {
            InitializeComponent();
            ViewModel = new SearchingPageViewModel();
            BindingContext = ViewModel;
        }

        private async void OnSearchRestaurantsButtonClicked(object sender, EventArgs e)
        {
            DishesLabel.IsVisible = false;

            ResultLabel.Text = "正在搜索推荐餐厅...";

            var token = await SecureStorage.Default.GetAsync("auth_token");
            string prompt = PromptEntry.Text;
            double? latitude = 31.231706;
            double? longitude = 121.472644;

            await ViewModel.GetRecommendedRestaurants(token, prompt, latitude, longitude);

            if (ViewModel.Restaurants.Count > 0)
            {
                RestaurantCollection.IsVisible = true;
                ResultLabel.Text = "来看看推荐的餐厅吧！";
            }
            else
            {
                ResultLabel.Text = "未找到推荐餐厅。";
            }
        }

        private async void OnSearchDishesButtonClicked(object sender, EventArgs e)
        {
            RestaurantCollection.IsVisible = false;

            ResultLabel.Text = "正在搜索推荐菜品...";

            var token = await SecureStorage.Default.GetAsync("auth_token");
            string prompt = PromptEntry.Text;

            await ViewModel.GetRecommendedDishes(token, prompt);

            if (!string.IsNullOrEmpty(ViewModel.Dishes))
            {
                DishesLabel.IsVisible = true;
                ResultLabel.Text = "来看看推荐的菜品吧！";
            }
            else
            {
                ResultLabel.Text = "未找到推荐菜品。";
            }
        }
    }
}
