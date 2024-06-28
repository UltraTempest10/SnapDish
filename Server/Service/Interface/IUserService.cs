using System;
using Dm.filter;
using Model.DTO;
using Model.Entity;

namespace Service.Interface
{
    public interface IUserService
    {
        public int CheckEmail(User user);

        public int Register(User user);

        LoginDTO Login(User user);

        UserProfile GetProfile(int userId);

        public int UpdateProfile(UserProfile newProfile);

        public int ChangePassword(int userId, PasswordDTO passwordDTO);
    }
}

