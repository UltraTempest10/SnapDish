using Microsoft.AspNetCore.Mvc;
using Server.config;
using Service.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService, JwtHelper jwtHelper)
        {
            _statisticsService = statisticsService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("overall-average")]
        [Authorize]
        public IActionResult GetOverallAvgRating([FromHeader(Name = "Authorization")] string token)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            double avgRating = _statisticsService.CalculateOverallAvgRating(user_id);
            if (avgRating < 0)
            {
                return NotFound("Rating not found");
            }
            return Ok(avgRating);
        }

        [HttpGet("monthly-average")]
        [Authorize]
        public IActionResult GetMonthlyAvgRating([FromHeader(Name = "Authorization")] string token, [FromQuery] int numMonth)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            var monthlyAvgRating = _statisticsService.CalculateMonthlyAvgRating(user_id, numMonth);
            if (monthlyAvgRating == null)
            {
                return NotFound("Rating not found");
            }
            return Ok(monthlyAvgRating);
        }

        [HttpGet("location-frequency")]
        [Authorize]
        public IActionResult GetLocationFrequency([FromHeader(Name = "Authorization")] string token)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            var locationFrequency = _statisticsService.CalculateLocationFrequency(user_id);
            if (locationFrequency == null)
            {
                return NotFound("Rating not found");
            }
            return Ok(locationFrequency);
        }

        [HttpGet("rating-distribution")]
        [Authorize]
        public IActionResult GetRatingDistribution([FromHeader(Name = "Authorization")] string token, [FromQuery] int minVal = 1, [FromQuery] int maxVal = 5)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            var ratingDistribution = _statisticsService.CalculateRatingDistribution(user_id, minVal, maxVal);
            if (ratingDistribution == null)
            {
                return NotFound("Rating not found");
            }
            return Ok(ratingDistribution);
        }
    }
}
