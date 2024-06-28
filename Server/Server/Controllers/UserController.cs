using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Dm.filter;
using Server.config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Entity;
using Model.Util;
using Service.Interface;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;
        private static Dictionary<string, string> resultMap = new Dictionary<string, string>();

        public UserController(IUserService service, JwtHelper jwtHelper)
        {
            _userService = service;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("send")]
        public IActionResult SendEmail([FromQuery] string email)
        {
            if (email == null)
                return BadRequest("Email is empty");

            string code = GenerateVerifyCode(6);
            bool success = SendEmailSMTP(email, "随食随记", "【随食随记】您的验证码为："
                + code + "（10分钟内有效）。如非本人操作，请忽略。");
            if (success)
            {
                SaveCode(code, email);
                return Ok("Send email successfully");
            }
            else
                return StatusCode(500, "Send email failed");
        }

        #region 邮箱验证相关
        private string GenerateVerifyCode(int length)
        {
            Random rad = new();
            int minValue = (int)Math.Pow(10, length - 1);
            int maxValue = (int)Math.Pow(10, length);
            int value = rad.Next(minValue, maxValue);

            // 转换为验证码
            string verifyCode = value.ToString();
            return verifyCode;
        }

        private void SaveCode(string code, string userEmail)
        {
            Console.WriteLine("code: " + code);
            // Console.WriteLine("userEmail: " + userEmail);
            // Console.WriteLine("resultMap count: " + resultMap.Count);

            DateTime currentTime = DateTime.Now.AddMinutes(10);
            string currentTimeString = currentTime.ToString("yyyyMMddHHmmss"); // 生成10分钟后时间，用户校验是否过期

            string hash = MD5Utils.Code(code); // 生成MD5值
            resultMap[userEmail + "hash"] = hash;
            resultMap[userEmail + "tamp"] = currentTimeString;

            // Console.WriteLine("resultMap count: " + resultMap.Count);
        }

        public static bool SendEmailSMTP(string mailTo, string mailSubject, string mailContent)
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress("snap.dish@foxmail.com")
            };
            try
            {
                mailMessage.To.Add(new MailAddress(mailTo));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Subject = mailSubject;
            mailMessage.Body = mailContent;
            // mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new()
            {
                Host = "smtp.qq.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential("snap.dish@foxmail.com", "zcemfkskuglrichd"),
                EnableSsl = true // SMTP服务器要求安全连接需要设置此属性
            };

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Send email successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send email failed: " + ex.Message);
                return false;
            }
        }
        #endregion

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user, [FromQuery] string code)
        {
            // 邮箱或密码或验证码有一个为空无法注册
            if (user.Email == null || user.Password == null || code == null)
                return BadRequest("Information is incomplete");

            // Console.WriteLine("输入验证码" + code);
            // Console.WriteLine("输入mail" + user.mail);

            // 判断验证码是否正确
            string requestHash, tamp;
            resultMap.TryGetValue(user.Email + "hash", out requestHash);
            resultMap.TryGetValue(user.Email + "tamp", out tamp);
            if (requestHash == null || tamp == null)
                return BadRequest("Verification code is invalid");

            DateTime currentTime = DateTime.Now;
            string currentTimeString = currentTime.ToString("yyyyMMddHHmmss"); // 当前时间

            if (tamp.CompareTo(currentTimeString) <= 0)
            {
                // 超时
                return StatusCode(500, "Verification code is invalid");
            }
            else
            {
                string hash = MD5Utils.Code(code); // 生成MD5值
                if (!hash.Equals(requestHash, StringComparison.OrdinalIgnoreCase))
                {
                    // 验证码不正确，校验失败
                    return StatusCode(500, "Verification code is incorrect");
                }
                else
                {
                    // 校验成功，检验邮箱是否注册
                    if (_userService.CheckEmail(user) > 0)
                        return StatusCode(500, "Email has been registered");
                    else
                    {
                        user.CreateTime = DateTime.Now;
                        user.Permission = 1; // 默认身份权限为普通用户
                        if (_userService.Register(user) != 1)
                            return StatusCode(500, "Failed to register");
                        else
                        {
                            // 注册成功，删除验证码
                            resultMap.Remove(user.Email + "hash");
                            resultMap.Remove(user.Email + "tamp");
                            return Ok("Register successfully");
                        }
                    }
                }
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.Email == null || user.Password == null)
                return BadRequest("Email or password is empty");

            var loginDTO = _userService.Login(user);
            if (loginDTO.Permission == null || loginDTO.Token == null)
                return StatusCode(500, "Login failed");

            int userId = int.Parse(loginDTO.Token);  // 前面UserService.Login返回的loginDTO.Token实际是userId，下面再转换成Token
            if (loginDTO.Permission == 1)
                loginDTO.Token = _jwtHelper.CreateToken(userId);
            else if (loginDTO.Permission == 0)
                loginDTO.Token = _jwtHelper.CreateToken(userId, 0);

            return Ok(loginDTO);
        }

        [HttpGet("profile")]
        [Authorize] 
        public IActionResult GetProfile([FromHeader(Name = "Authorization")] string token)
        {
            int userId = _jwtHelper.GetUserIdFromToken(token);
            // Console.WriteLine("userId:"+userId);

            UserProfile userProfileDTO = _userService.GetProfile(userId);

            Console.WriteLine("userProfileDTO:" + JObject.FromObject(userProfileDTO).ToString());

            if (userProfileDTO == null)
                return StatusCode(500, "Failed to get profile");

            return Ok(userProfileDTO);
        }

        [HttpPut("update")]
        [Authorize]
        public IActionResult UpdateSelfProfile([FromHeader(Name = "Authorization")] string token, [FromBody] UserProfile newProfile)
        {
            int userId = _jwtHelper.GetUserIdFromToken(token);

            Console.WriteLine("userId:" + userId);

            newProfile.UserId = userId;

            if (_userService.UpdateProfile(newProfile) != 1)
                return StatusCode(500, "Failed to update profile");

            return Ok("Update profile successfully");
        }

        /*修改密码*/
        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromHeader(Name = "Authorization")] string token, [FromBody] PasswordDTO passwordDTO)
        {
            int userId = _jwtHelper.GetUserIdFromToken(token);

            int result = _userService.ChangePassword(userId, passwordDTO);
            if (result == -1)
                return StatusCode(500, "Password is empty");
            else if (result == -2)
                return StatusCode(500, "Old password is incorrect");
            else if (result == 1)
                return Ok("Change password successfully");
            else
                return StatusCode(500, "Failed to change password");
        }
    }
}
