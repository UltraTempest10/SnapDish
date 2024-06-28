using Model.Entity;

namespace Service.Interface
{
    public interface IStatisticsService
    {
        public double CalculateOverallAvgRating(int userId);

        public Dictionary<string, double> CalculateMonthlyAvgRating(int userId, int numMonth);

        public Dictionary<string, int> CalculateLocationFrequency(int userId);

        public List<int> CalculateRatingDistribution(int userId, int minVal, int maxVal);
    }
}
