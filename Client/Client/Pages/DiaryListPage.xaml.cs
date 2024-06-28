using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

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
    }

    public class DiaryListPageViewModel
    {
        public ObservableCollection<Diary> FoodRecords { get; set; }

        public DiaryListPageViewModel()
        {
            FoodRecords = new ObservableCollection<Diary>();
            LoadFoodRecords();
        }

        private void LoadFoodRecords()
        {
            // 在此处加载数据。此示例中，添加了几个示例记录。
            FoodRecords.Add(new Diary
            {
                Title = "麻辣火锅",
                Location = "成都",
                Content = "非常美味的麻辣火锅",
                Date = DateTime.Now
            });
            FoodRecords.Add(new Diary
            {
                Title = "北京烤鸭",
                Location = "北京",
                Content = "经典的北京烤鸭",
                Date = DateTime.Now
            });
        }
    }

}
