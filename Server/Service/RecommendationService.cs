using Service.Interface;
using SqlSugar;
using Model.Entity;
using Model.DTO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ISqlSugarClient _db;
        private readonly AiService _aiService = new();

        public RecommendationService(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<RestaurantBriefDTO>> GetRecommendedRestaurants(int userId, string prompt, double latitude, double longitude)
        {
            // 获取当前用户评分前10的日记
            List<Diary> topDiaries = _db.Queryable<Diary>()
                                        .Where(d => d.UserId == userId && d.Rating.HasValue)
                                        .OrderBy(d => d.Rating, OrderByType.Desc)
                                        .Take(10)
                                        .ToList();

            // 将日记内容拼接成一个字符串
            string diaryText = string.Join(" ", topDiaries.Select(d => d.Content));

            // 细化用户的需求
            string refinedPrompt = $"向我推荐3个餐厅类别，基于我的日记：“{diaryText}”。我的附加偏好：“{prompt}”。可选的类别包括：重庆火锅, 日本料理, 日式烧烤/烤肉, 火锅, 自助餐, 杭帮菜, 饮品, 意大利菜, 本帮菜, 本帮江浙菜, 港台甜品, 私房菜, 西餐, 寿司, 蟹宴, 烧烤, 粤菜馆, 西班牙菜, 西式甜点, 面馆, 云南菜, 潮汕菜, 涮羊肉, 咖啡厅, 韩国料理, 闽菜, 其他中餐, 西式简餐, 串串香, 川菜/家常菜, 面包甜点, 日式铁板烧, 淮扬菜, 创意菜, 北京菜, 素菜, 海鲜, 快餐简餐, 甜品饮品, 牛排, 四川火锅, 老北京小吃, 烤鸭, 其他美食, 泰国菜, 湘菜, 俄罗斯菜, 鱼火锅, 融合菜, 内蒙菜, 法国菜, 日式简餐/快餐, 江浙菜, 雪糕饮品, 粤菜, 新式甜品, 小龙虾, 台湾菜, 越南菜, 东南亚菜, 川菜, 茶餐厅, 顺德菜, 川味火锅/麻辣火锅, 牛肉火锅, 粥粉面, 比萨, 小吃快餐, 西北菜, 天津菜, 上海江浙, 冰淇淋, 上海菜, 新疆菜, 小吃, 津味小吃, 新加坡菜, 江浙小吃, 杭帮/江浙菜, 日式面条, 南京/江浙菜, 南京菜/家常菜, 麻辣烫, 馄饨, 小火锅, 西式正餐, 苏帮菜, 苏州江浙, 生煎/锅贴, 大闸蟹, 钵钵鸡, 烤鱼, 兔头/兔丁, 云贵菜, 抄手, 蹄花, 湖北菜/家常菜, 热干面, 粉面馆, 江湖菜, 陕菜, 泡馍, 肉夹馍, 水盆羊肉, 胡辣汤。从可选的类别中选择3个，每个类别前加一个竖线（|）。除了3个类别之外，不要回复其他文字。";

            // 调用AI服务获取推荐的餐厅类别
            string recommendedCategories = await _aiService.GetAnswerAsync(refinedPrompt);

            // 使用正则表达式匹配竖线分隔的三个词
            string pattern = @"\|([^|]+)\|([^|]+)\|([^|]+)";
            Match match = Regex.Match(recommendedCategories, pattern);
            string[] categories = new string[3];
            for (int i = 1; i <= 3; i++)
            {
                categories[i - 1] = match.Groups[i].Value.Trim();
            }

            // 获取当前城市
            string city = CityService.GetCity(latitude, longitude);

            // Console.WriteLine($"City: {city}");

            // 获取推荐的餐厅，根据口味评分、环境评分、服务评分排序，取前3个，返回简要信息
            List<RestaurantBriefDTO> restaurants = await _db.Queryable<Restaurant>()
                .Where(r => categories.Contains(r.mainCategoryName) && r.city == city)
                .OrderBy(r => r.tasteScore, OrderByType.Desc)
                .OrderBy(r => r.environmentScore, OrderByType.Desc)
                .OrderBy(r => r.serviceScore, OrderByType.Desc)
                .Take(3)
                .Select(r => new RestaurantBriefDTO
                {
                    Name = r.shopName,
                    Category = r.mainCategoryName,
                    tasteScore = r.tasteScore,
                    environmentScore = r.environmentScore,
                    serviceScore = r.serviceScore,
                    avgPrice = r.avgPrice,
                    Address = r.shopAddress,
                    Image = r.defaultPic
                })
                .ToListAsync();

            // 如果没有推荐的餐厅，则取消类别限制，重新获取
            if (restaurants.Count == 0)
            {
                restaurants = await _db.Queryable<Restaurant>()
                    .Where(r => r.city == city)
                    .OrderBy(r => r.tasteScore, OrderByType.Desc)
                    .OrderBy(r => r.environmentScore, OrderByType.Desc)
                    .OrderBy(r => r.serviceScore, OrderByType.Desc)
                    .Take(3)
                    .Select(r => new RestaurantBriefDTO
                    {
                        Name = r.shopName,
                        Category = r.mainCategoryName,
                        tasteScore = r.tasteScore,
                        environmentScore = r.environmentScore,
                        serviceScore = r.serviceScore,
                        avgPrice = r.avgPrice,
                        Address = r.shopAddress,
                        Image = r.defaultPic
                    })
                    .ToListAsync();
            }

            return restaurants;
        }

        public Task<string> GetRecommendedDishes(int userId, string prompt)
        {
            // 获取当前用户评分前10的日记
            List<Diary> topDiaries = _db.Queryable<Diary>()
                                        .Where(d => d.UserId == userId && d.Rating.HasValue)
                                        .OrderBy(d => d.Rating, OrderByType.Desc)
                                        .Take(10)
                                        .ToList();

            // 将日记内容拼接成一个字符串
            string diaryText = string.Join(" ", topDiaries.Select(d => d.Content));

            // 细化用户的需求
            string refinedPrompt = $"向我推荐2道美味的菜品，基于我的日记：“{diaryText}”。我的附加偏好：“{prompt}”。以“今日份的美食推荐”或类似的话作为你回复的开头。除了2道菜品及其简短介绍，不要回复其他内容。";
            
            // 调用AI服务获取推荐的菜品
            return _aiService.GetAnswerAsync(refinedPrompt);
        }
    }
}
