using HamrahFelez.Repositories;
using HamrahFelez.ViewModels;

namespace HamrahFelez.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<View_WebUser?> GetUserByUsernameAsync(GetUserByUsernameRequest request)
        {
            return _userRepository.GetUserByUsernameAsync(request.Username.Trim().ToLowerInvariant());
        }
    }
}
