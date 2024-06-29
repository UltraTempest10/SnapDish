using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SnapDishApp
{
    public class DiaryPageViewModel : BindableObject
    {
        private DateTime _selectedDate;
        private string _content;

        public DiaryPageViewModel()
        {
            SaveDiaryEntryCommand = new Command(SaveDiaryEntry);
            SelectedDate = DateTime.Now; // 设置默认日期为当前日期
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveDiaryEntryCommand { get; }

        private void SaveDiaryEntry()
        {
            // 实现保存逻辑
            // 比如，将日记条目保存到数据库或文件
            Application.Current.MainPage.DisplayAlert("Saved", "Your diary entry has been saved.", "OK");
        }
    }
}
