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
                PickerTitle = "��ѡ��һ��ͼƬ"
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
                StatusLabel.Text = "ͼƬ��ǿ��...";
                string response = await UploadImageAsync(url, _selectedImageStream, _selectedImageName);
                // StatusLabel.Text = response != null ? "ͼƬ��ǿ�ɹ�!" : "ͼƬ��ǿʧ��!";
                StatusLabel.Text = response != null ? response : "ͼƬ��ǿʧ��!";
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

                    // ����ǿ���ͼƬ�����ݸ���ʾ����
                    ReplaceOriginalImage(responseStream);
                    return "Image enhanced and replaced";
                }
            }
            catch (HttpRequestException e)
            {
                // �����쳣
                Console.WriteLine($"Request error: {e.Message}");
                return "Upload failed";
            }
        }

        private void ReplaceOriginalImage(Stream imageStream)
        {
            // �滻ԭͼ
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

            // �����¼�����ݿ�򱾵ش洢
            // �˴������Ϊʾ��������ʵ����Ҫ����ʵ�������д

            await DisplayAlert("����ɹ�", "��ʳ��¼�ѱ���", "ȷ��");
            await Navigation.PopAsync();
        }
    }
}
