using Model.DTO;
using Model.Entity;

namespace Service.Interface
{
    public interface IRecommendationService
    {
        public Task<List<RestaurantBriefDTO>> GetRecommendedRestaurants(int userId, string prompt, double latitude, double longitude);

        public Task<string> GetRecommendedDishes(int userId, string prompt);
    }
}
