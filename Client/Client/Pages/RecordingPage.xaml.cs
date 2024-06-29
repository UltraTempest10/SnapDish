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
                PickerTitle = "请选择一张图片"
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
                StatusLabel.Text = "图片增强中...";
                string response = await UploadImageAsync(token, url, _selectedImagePath, _selectedImageName);
                // StatusLabel.Text = response != null ? "图片增强成功!" : "图片增强失败!";
                StatusLabel.Text = response != null ? response : "图片增强失败!";
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

                                // 将增强后的图片流暂存到文件
                                string tempFilePath = Path.Combine(Path.GetTempPath(), "enhanced_" + fileName);
                                _selectedImagePath = tempFilePath;
                                await SaveStreamToFile(responseStream, tempFilePath);

                                // 将增强后的图片流传递给显示方法
                                _selectedImageStream = File.OpenRead(tempFilePath);
                                _selectedImageName = "enhanced_" + fileName;
                                ReplaceOriginalImage(_selectedImageStream);

                                return "图片增强成功";
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // 处理异常
                Console.WriteLine($"Request error: {e.Message}");
                return "上传失败";
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
                await DisplayAlert("保存成功", "美食记录已保存", "确定");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("保存失败", "美食记录保存失败", "确定");
            }
        }

        private async Task<string> UploadDiaryFormDataAsync(string token, string url, string title, string location, string content, int rating, string imagePath, string fileName, int? diaryId = null)
        {
            try
            {
                using (var form = new MultipartFormDataContent())
                {
                    // 添加文本字段
                    form.Add(new StringContent(title), "Title");
                    form.Add(new StringContent(location), "Location");
                    form.Add(new StringContent(content), "Content");
                    form.Add(new StringContent(rating.ToString()), "Rating");

                    if (diaryId.HasValue)
                    {
                        form.Add(new StringContent(diaryId.Value.ToString()), "DiaryId");
                    }

                    // 添加文件字段
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

                        return "上传成功: " + responseBody;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // 处理异常
                Console.WriteLine($"Request error: {e.Message}");
                return "上传失败";
            }
            catch (Exception e)
            {
                // 处理其他异常
                Console.WriteLine($"General error: {e.Message}");
                return "上传失败";
            }
        }
    }
}
