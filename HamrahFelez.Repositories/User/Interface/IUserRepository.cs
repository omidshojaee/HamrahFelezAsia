using HamrahFelez.ViewModels;

namespace HamrahFelez.Repositories
{
    public interface IUserRepository
    {
        public Task<View_WebUser> GetUserByUsernameAsync(string username);
    }
}
