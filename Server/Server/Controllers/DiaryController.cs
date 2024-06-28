using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Model.Entity;
using Model.DTO;
using Server.config;
using Service.Interface;
using NetTaste;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IDiaryService _diaryService;

        public DiaryController(IDiaryService diaryService, JwtHelper jwtHelper)
        {
            _diaryService = diaryService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult GetDiaryList([FromHeader(Name = "Authorization")] string token)
        {
            int user_id = _jwtHelper.GetUserIdFromToken(token);
            var diaries = _diaryService.GetDiaryList(user_id);
            return Ok(diaries);
        }

        [HttpGet("detail")]
        [Authorize]
        public IActionResult GetDiary([FromQuery] int diaryId)
        {
            var diary = _diaryService.GetDiary(diaryId);
            if (diary == null)
            {
                return NotFound("Diary not found");
            }
            return Ok(diary);
        }

        [HttpPost("add")]
        [Authorize]
        public IActionResult AddDiary([FromHeader(Name = "Authorization")] string token, [FromForm] DiaryFormData diaryFormData)
        {
            int userId = _jwtHelper.GetUserIdFromToken(token);

            Diary diary = new()
            {
                UserId = userId,
                Title = diaryFormData.Title,
                Location = diaryFormData.Location,
                Content = diaryFormData.Content,
                Rating = diaryFormData.Rating,
                CreateTime = DateTime.Now
            };

            // 先保存图片到本地
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "temp", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string extension = Path.GetExtension(diaryFormData.Image.FileName);
            string imagePath = Path.Combine(directoryPath, "diary_" + userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                diaryFormData.Image.CopyTo(stream);
            }

            if (_diaryService.AddDiary(diary, imagePath) != 1)
            {
                return StatusCode(500, "Failed to add diary");
            }
            return Ok("Add diary successfully");
        }

        [HttpPost("enhance")]
        [Authorize]
        public async Task<IActionResult> EnhanceImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "temp", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string extension = Path.GetExtension(file.FileName);
                string inputPath = Path.Combine(directoryPath, "input" + extension);
                using (var stream = new FileStream(inputPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                string outputPath = Path.Combine(directoryPath, "output" + extension);

                // 调用增强图片的方法
                if (_diaryService.Enhance(inputPath, outputPath) != 0)
                {
                    return StatusCode(500, "Failed to enhance image");
                }

                var enhancedImageStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
                return File(enhancedImageStream, "image/jpeg");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while enhancing the image");
            }
        }

        [HttpPut("update")]
        [Authorize]
        public IActionResult UpdateDiary([FromHeader(Name = "Authorization")] string token, [FromForm] DiaryFormData diaryFormData)
        {
            if (diaryFormData.DiaryId == null)
            {
                return BadRequest("DiaryId is empty");
            }

            int userId = _jwtHelper.GetUserIdFromToken(token);

            Diary diary = new()
            {
                UserId = userId,
                DiaryId = (int)diaryFormData.DiaryId,
                Title = diaryFormData.Title,
                Location = diaryFormData.Location,
                Content = diaryFormData.Content,
                Rating = diaryFormData.Rating
            };

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "temp", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string extension = Path.GetExtension(diaryFormData.Image.FileName);
            string imagePath = Path.Combine(directoryPath, "diary_" + userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                diaryFormData.Image.CopyTo(stream);
            }

            if (_diaryService.UpdateDiary(diary, imagePath) != 1)
            {
                return StatusCode(500, "Failed to update diary");
            }
            return Ok("Update diary successfully");
        }

        [HttpDelete("delete")]
        [Authorize]
        public IActionResult DeleteDiary([FromQuery] int diaryId)
        {
            if (_diaryService.DeleteDiary(diaryId) != 1)
            {
                return StatusCode(500, "Failed to delete diary");
            }
            return Ok("Delete diary successfully");
        }
    }

    public class DiaryFormData
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public IFormFile Image { get; set; }
        public int? DiaryId { get; set; }
    }
}
