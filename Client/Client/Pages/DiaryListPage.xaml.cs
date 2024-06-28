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
            // �ڴ˴��������ݡ���ʾ���У�����˼���ʾ����¼��
            FoodRecords.Add(new Diary
            {
                Title = "�������",
                Location = "�ɶ�",
                Content = "�ǳ���ζ���������",
                Date = DateTime.Now
            });
            FoodRecords.Add(new Diary
            {
                Title = "������Ѽ",
                Location = "����",
                Content = "����ı�����Ѽ",
                Date = DateTime.Now
            });
        }
    }

}
