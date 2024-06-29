using System;
using Microsoft.Maui.Controls;

namespace SnapDishApp
{
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPageViewModel ViewModel { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();
            ViewModel = new StatisticsPageViewModel();
            BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var token = await SecureStorage.Default.GetAsync("auth_token");
            int numMonth = 3; // Example: last 4 months

            await ViewModel.GetStatisticsAsync(token, numMonth);
        }
    }
}
