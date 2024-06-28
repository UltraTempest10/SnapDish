using System;
using System.Data;
using System.Numerics;
using Dm.filter;
using Model.DTO;
using Model.Entity;
using Service.Interface;
using Newtonsoft.Json.Linq;
using SqlSugar;
using static System.Formats.Asn1.AsnWriter;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly ISqlSugarClient _db;
        public UserService(ISqlSugarClient db)
        {
            _db = db;
        }

        public int CheckEmail(User user)
        {
            List<User> list = _db.Queryable<User>().Where(it => it.Email == user.Email).ToList();
            return list.Count;
        }

        public int Register(User user)
        {
            try
            {
                return _db.Insertable(user).ExecuteReturnIdentity();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public LoginDTO Login(User user)
        {
            LoginDTO loginDTO = new();
            var userGet = _db.Queryable<User>().First(it => it.Email == user.Email && it.Password == user.Password);

            if (userGet == null)
                return loginDTO;

            loginDTO.Token = userGet.UserId.ToString();
            loginDTO.Permission = userGet.Permission;

            return loginDTO;
        }

        public UserProfile GetProfile(int userId)
        {
            UserProfile profile = _db.Queryable<UserProfile>().First(it => it.UserId == userId);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Nickname = "未设置",
                    Location = "未设置",
                    Introduction = "未设置"
                };
                _db.Insertable(profile).ExecuteCommand();
            }
            return profile;
        }

        public int UpdateProfile(UserProfile newProfile)
        {
            var result = _db.Updateable<UserProfile>()
                .SetColumns(it => new UserProfile
                {
                    Nickname = newProfile.Nickname,
                    Location = newProfile.Location,
                    Introduction = newProfile.Introduction
                })
                .Where(it => it.UserId == newProfile.UserId)
                .ExecuteCommand();

            Console.WriteLine("UpdateProfile result: " + result);

            return result;
        }
        
        public int ChangePassword(int userId, PasswordDTO passwordDTO)
        {
            var user = _db.Queryable<User>().First(it => it.UserId == userId);
            if (user.Password == null)
                return -1;
            string oldPassword = user.Password;
            if (oldPassword.Equals(passwordDTO.OldPassword))
            {
                var result = _db.Updateable<User>()
                .SetColumns(it => new User
                {
                    Password = passwordDTO.NewPassword
                })
                .Where(it => it.UserId == userId)
                .ExecuteCommand();

                return result;
            }
            else
            {
                Console.WriteLine("Old password is wrong");
                return -2;
            }
        }
    }
}
