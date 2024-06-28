using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Configs
{
    public static class HttpClientInstance
    {
        public static readonly HttpClient HttpClient = new();

        public static string BaseAddress =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:9876" : "http://localhost:9876";

        public static string DiaryEnhanceUrl = $"{BaseAddress}/Diary/enhance";
    }
}
