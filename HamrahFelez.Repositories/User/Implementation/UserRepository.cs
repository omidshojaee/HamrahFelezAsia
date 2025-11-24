using HamrahFelez.ViewModels.User;

using HamrahFelez.Repository.User.Interface;
using HamrahFelez.Repository.DataAccess;

namespace HamrahFelez.Repository.User.Implementation
{
    public class UserRepository : IUserRepository
    {
        public async Task<View_WebUser> GetUserByUsernameAsync(string where, object whereParams)
        {
            var select = "SELECT TOP (1) * FROM View_WebUser";

            var result = await DataAccessManager.GetFromViewAsync<View_WebUser>
                (
                    select,
                    where,
                    whereParams
                );

            return result.FirstOrDefault();
        }
    }
}
