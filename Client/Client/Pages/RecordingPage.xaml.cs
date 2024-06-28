using Client.Configs;
using System.Net.Http.Headers;

namespace SnapDishApp
{
    public partial class RecordingPage : ContentPage
    {
        private Stream _selectedImageStream;
        private string _selectedImageName;

        public RecordingPage()
        {
            InitializeComponent();
        }

        private async void OnSelectImageClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "请选择一张图片"
            });

            if (result != null)
            {
                _selectedImageStream = await result.OpenReadAsync();
                _selectedImageName = result.FileName;

                SelectedImage.Source = ImageSource.FromStream(() => _selectedImageStream);
                SelectedImage.IsVisible = true;
                SelectImageBorder.IsVisible = false;
                EnhanceButton.IsVisible = true;
            }
        }

        private async void OnEnhanceButtonClicked(object sender, EventArgs e)
        {
            if (_selectedImageStream != null)
            {
                string url = HttpClientInstance.DiaryEnhanceUrl;
                StatusLabel.Text = "图片增强中...";
                string response = await UploadImageAsync(url, _selectedImageStream, _selectedImageName);
                // StatusLabel.Text = response != null ? "图片增强成功!" : "图片增强失败!";
                StatusLabel.Text = response != null ? response : "图片增强失败!";
            }
        }

        public async Task<string> UploadImageAsync(string url, Stream imageStream, string fileName)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    var imageContent = new StreamContent(imageStream);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "file", fileName);

                    HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    var responseStream = await response.Content.ReadAsStreamAsync();

                    // 将增强后的图片流传递给显示方法
                    ReplaceOriginalImage(responseStream);
                    return "Image enhanced and replaced";
                }
            }
            catch (HttpRequestException e)
            {
                // 处理异常
                Console.WriteLine($"Request error: {e.Message}");
                return "Upload failed";
            }
        }

        private void ReplaceOriginalImage(Stream imageStream)
        {
            // 替换原图
            Device.BeginInvokeOnMainThread(() =>
            {
                SelectedImage.Source = ImageSource.FromStream(() => imageStream);
            });
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var foodRecord = new Diary
            {
                Title = FoodNameEntry.Text,
                Location = LocationEntry.Text,
                Content = DescriptionEditor.Text,
                Date = DateTime.Now
            };

            // 保存记录到数据库或本地存储
            // 此处代码仅为示例，具体实现需要根据实际情况编写

            await DisplayAlert("保存成功", "美食记录已保存", "确定");
            await Navigation.PopAsync();
        }
    }
}
