using Microsoft.AspNetCore.Mvc;
using Server.config;
using Service.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService, JwtHelper jwtHelper)
        {
            _recommendationService = recommendationService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("restaurant")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedRestaurants([FromHeader(Name = "Authorization")] string token, [FromQuery] string prompt, [FromQuery] double? latitude, [FromQuery] double? longitude)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);

            Console.WriteLine("latitude: " + latitude);
            Console.WriteLine("longitude: " + longitude);
            
            bool hasLocation = latitude != null && longitude != null;
            double lat = hasLocation ? (double)latitude : 31.231706;
            double lon = hasLocation ? (double)longitude : 121.472644;

            var restaurants = await _recommendationService.GetRecommendedRestaurants(user_id, prompt, lat, lon);
            if (restaurants == null)
            {
                return NotFound("Recommendation not found");
            }
            return Ok(new { restaurants });
        }

        [HttpGet("dish")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedDishes([FromHeader(Name = "Authorization")] string token, [FromQuery] string prompt)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            var dishes = await _recommendationService.GetRecommendedDishes(user_id, prompt);
            if (dishes == null)
            {
                return NotFound("Recommendation not found");
            }
            return Ok(new { dishes });
        }
    }
}
