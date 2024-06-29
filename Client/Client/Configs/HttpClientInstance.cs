using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapDishApp
{
    public static class HttpClientInstance
    {
        public static readonly HttpClient HttpClient = new();

        public static string BaseAddress = "http://121.196.246.253:9876";

        public static string UserUrl = $"{BaseAddress}/User";

        public static string UserSendUrl = $"{UserUrl}/send";

        public static string UserLoginUrl = $"{UserUrl}/login";

        public static string UserRegisterUrl = $"{UserUrl}/register";

        public static string UserProfileUrl = $"{UserUrl}/profile";

        public static string UserUpdateUrl = $"{UserUrl}/update";

        public static string UserChangePasswordUrl = $"{UserUrl}/change-password";

        public static string DiaryUrl = $"{BaseAddress}/Diary";

        public static string DiaryListUrl = $"{DiaryUrl}/list";

        public static string DiaryDetailUrl = $"{DiaryUrl}/detail";

        public static string DiaryAddUrl = $"{DiaryUrl}/add";

        public static string DiaryEnhanceUrl = $"{DiaryUrl}/enhance";

        public static string DiaryUpdateUrl = $"{DiaryUrl}/update";

        public static string DiaryDeleteUrl = $"{DiaryUrl}/delete";

        public static string RecommendationUrl = $"{BaseAddress}/Recommendation";

        public static string RecommendationRestaurantUrl = $"{RecommendationUrl}/restaurant";

        public static string RecommendationDishUrl = $"{RecommendationUrl}/dish";

        public static string StatisticsUrl = $"{BaseAddress}/Statistics";

        public static string StatisticsOverallUrl = $"{StatisticsUrl}/overall-average";

        public static string StatisticsMonthlyUrl = $"{StatisticsUrl}/monthly-average";

        public static string StatisticsLocationUrl = $"{StatisticsUrl}/location-frequency";

        public static string StatisticsDistributionUrl = $"{StatisticsUrl}/rating-distribution";
    }
}
