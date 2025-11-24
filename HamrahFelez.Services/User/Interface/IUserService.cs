using HamrahFelez.ViewModels.User;

namespace HamrahFelez.Services.User.Interface
{
    public interface IUserService
    {
        Task<View_WebUser> GetUserByUsernameAsync(GetUserByUsernameRequest model);
    }
}
