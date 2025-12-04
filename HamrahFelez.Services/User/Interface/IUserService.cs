using HamrahFelez.ViewModels;

namespace HamrahFelez.Services
{
    public interface IUserService
    {
        Task<View_WebUser> GetUserByUsernameAsync(GetUserByUsernameRequest request);
    }
}
