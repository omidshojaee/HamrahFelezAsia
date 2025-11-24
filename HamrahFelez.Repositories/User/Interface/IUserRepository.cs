using HamrahFelez.ViewModels.User;

namespace HamrahFelez.Repository.User.Interface
{
    public interface IUserRepository
    {
        public Task<View_WebUser> GetUserByUsernameAsync(string where, object whereParams);
    }
}
