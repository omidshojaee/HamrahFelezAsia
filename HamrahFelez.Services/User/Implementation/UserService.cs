using HamrahFelez.Repository.User.Interface;
using HamrahFelez.Services.User.Interface;
using HamrahFelez.ViewModels.User;

namespace HamrahFelez.Services.User.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<View_WebUser> GetUserByUsernameAsync(GetUserByUsernameRequest model)
        {
            var where = "WHERE Username = @Username";
            var whereParams = new { Username = model.Username };

            return _userRepository.GetUserByUsernameAsync(where, whereParams);
        }
    }
}
