using SnapDishApp;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SnapDishApp
{
    public partial class RecordingPage : ContentPage
    {
        private Stream? _selectedImageStream;
        private string? _selectedImagePath;
        private string? _selectedImageName;

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
                _selectedImagePath = result.FullPath;
                _selectedImageName = result.FileName;

                SelectedImage.Source = ImageSource.FromStream(() => _selectedImageStream);
                SelectedImage.IsVisible = true;
                SelectImageBorder.IsVisible = false;
                EnhanceButton.IsVisible = true;
            }
        }

        private async void OnEnhanceButtonClicked(object sender, EventArgs e)
        {
            var token = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_token")).Result;

            if (_selectedImageStream != null)
            {
                string url = HttpClientInstance.DiaryEnhanceUrl;
                StatusLabel.Text = "ͼƬ��ǿ��...";
                string response = await UploadImageAsync(token, url, _selectedImagePath, _selectedImageName);
                // StatusLabel.Text = response != null ? "ͼƬ��ǿ�ɹ�!" : "ͼƬ��ǿʧ��!";
                StatusLabel.Text = response != null ? response : "ͼƬ��ǿʧ��!";
            }
        }

        public async Task<string> UploadImageAsync(string token, string url, string imagePath, string fileName)
        {
            try
            {
                using (var form = new MultipartFormDataContent())
                {
                    //Console.WriteLine("token: " + token);
                    //Console.WriteLine("url: " + url);
                    //Console.WriteLine("imagePath: " + imagePath);
                    //Console.WriteLine("fileName: " + fileName);
                    using (var fs = File.OpenRead(imagePath))
                    {
                        //Console.WriteLine("fs: " + fs);
                        using (var streamContent = new StreamContent(fs))
                        {
                            //Console.WriteLine("streamContent: " + streamContent);
                            using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                            {
                                //Console.WriteLine("fileContent: " + fileContent);

                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                                // "file" parameter name should be the same as the server side input parameter name
                                form.Add(fileContent, "file", fileName);
                                HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(url, form);

                                response.EnsureSuccessStatusCode();
                                var responseStream = await response.Content.ReadAsStreamAsync();

                                var fileStream = responseStream;

                                // ����ǿ���ͼƬ���ݴ浽�ļ�
                                string tempFilePath = Path.Combine(Path.GetTempPath(), "enhanced_" + fileName);
                                _selectedImagePath = tempFilePath;
                                await SaveStreamToFile(responseStream, tempFilePath);

                                // ����ǿ���ͼƬ�����ݸ���ʾ����
                                _selectedImageStream = File.OpenRead(tempFilePath);
                                _selectedImageName = "enhanced_" + fileName;
                                ReplaceOriginalImage(_selectedImageStream);

                                return "ͼƬ��ǿ�ɹ�";
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // �����쳣
                Console.WriteLine($"Request error: {e.Message}");
                return "�ϴ�ʧ��";
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

        private async Task SaveStreamToFile(Stream stream, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var token = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_token")).Result;

            var diary = new Diary
            {
                Title = TitleEntry.Text,
                Location = LocationEntry.Text,
                Content = ContentEditor.Text,
                Rating = RatingPicker.SelectedIndex + 1
            };

            string url = HttpClientInstance.DiaryAddUrl;
            string response = await UploadDiaryFormDataAsync(token, url, diary.Title, diary.Location, diary.Content, diary.Rating, _selectedImagePath, _selectedImageName);

            if (response != null)
            {
                await DisplayAlert("����ɹ�", "��ʳ��¼�ѱ���", "ȷ��");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("����ʧ��", "��ʳ��¼����ʧ��", "ȷ��");
            }
        }

        private async Task<string> UploadDiaryFormDataAsync(string token, string url, string title, string location, string content, int rating, string imagePath, string fileName, int? diaryId = null)
        {
            try
            {
                using (var form = new MultipartFormDataContent())
                {
                    // ����ı��ֶ�
                    form.Add(new StringContent(title), "Title");
                    form.Add(new StringContent(location), "Location");
                    form.Add(new StringContent(content), "Content");
                    form.Add(new StringContent(rating.ToString()), "Rating");

                    if (diaryId.HasValue)
                    {
                        form.Add(new StringContent(diaryId.Value.ToString()), "DiaryId");
                    }

                    // ����ļ��ֶ�
                    using (var fs = File.OpenRead(imagePath))
                    using (var streamContent = new StreamContent(fs))
                    using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                    {
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        form.Add(fileContent, "Image", fileName);

                        HttpClientInstance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        HttpResponseMessage response = await HttpClientInstance.HttpClient.PostAsync(url, form);

                        response.EnsureSuccessStatusCode();
                        var responseBody = await response.Content.ReadAsStringAsync();

                        Console.WriteLine("responseBody: " + responseBody);

                        return "�ϴ��ɹ�: " + responseBody;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // �����쳣
                Console.WriteLine($"Request error: {e.Message}");
                return "�ϴ�ʧ��";
            }
            catch (Exception e)
            {
                // ���������쳣
                Console.WriteLine($"General error: {e.Message}");
                return "�ϴ�ʧ��";
            }
        }
    }
}
