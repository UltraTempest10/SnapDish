using Service.Interface;
using SqlSugar;
using Model.Entity;
using Model.DTO;
using DataAnalyzer;

namespace Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ISqlSugarClient _db;

        public StatisticsService(ISqlSugarClient db)
        {
            _db = db;
        }

        // 计算当前用户所有日记的平均评分
        public double CalculateOverallAvgRating(int userId)
        {
            List<double> ratings = _db.Queryable<Diary>()
                                      .Where(d => d.UserId == userId && d.Rating.HasValue)
                                      .Select(d => d.Rating.Value * 1.0)
                                      .ToList();

            // foreach (var rating in ratings)
            // {
                // Console.WriteLine(rating);
            // }

            return Analyzer.CalculateOverallAvg(ratings);
        }

        // 计算当前用户近n个月的平均评分
        public Dictionary<string, double> CalculateMonthlyAvgRating(int userId, int numMonths)
        {
            if (numMonths <= 0)
            {
                return null;
            }

            Dictionary<string, double> monthlyAvg = new();

            for (int i = 0; i < numMonths; i++)
            {
                DateTime startDate = DateTime.Now.AddMonths(-i - 1);
                DateTime endDate = DateTime.Now.AddMonths(-i);

                // Console.WriteLine(startDate);
                // Console.WriteLine(endDate);

                List<double> ratings = _db.Queryable<Diary>()
                                          .Where(d => d.UserId == userId && d.Rating.HasValue && d.CreateTime >= startDate && d.CreateTime < endDate)
                                          .Select(d => d.Rating.Value * 1.0)
                                          .ToList();

                // foreach (var rating in ratings)
                // {
                    // Console.WriteLine(rating);
                // }

                double avgRating = Analyzer.CalculateOverallAvg(ratings);
                monthlyAvg[startDate.ToString("yyyy-MM")] = avgRating;
            }
            return monthlyAvg;
        }

        // 计算当前用户每个位置的日记频率
        public Dictionary<string, int> CalculateLocationFrequency(int userId)
        {
            List<string> locations = _db.Queryable<Diary>()
                                        .Where(d => d.UserId == userId && d.Location != null)
                                        .Select(d => d.Location)
                                        .ToList();
            var categoryFrequency = Analyzer.CalculateCategoryFrequency(locations);
            return categoryFrequency.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // 计算当前用户评分的分布
        public List<int> CalculateRatingDistribution(int userId, int minVal, int maxVal)
        {
            List<int> ratings = _db.Queryable<Diary>()
                                      .Where(d => d.UserId == userId && d.Rating.HasValue)
                                      .Select(d => d.Rating.Value)
                                      .ToList();

            return Analyzer.CalculateValueDistribution(ratings, minVal, maxVal);
        }
    }
}
