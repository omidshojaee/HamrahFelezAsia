using HamrahFelez.ViewModels;

namespace HamrahFelez.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<View_WebUser?> GetUserByUsernameAsync(string username)
        {
            var select = "SELECT TOP (1) * FROM View_WebUser";
            var where = "WHERE Username = @Username";
            var whereParams = new { Username = username };

            var result = await DataAccessManager.GetFromViewAsync<View_WebUser>(
                select,
                where,
                whereParams);

            return result.FirstOrDefault();
        }
    }
}
