using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<int> RegisterUser(User user);
        Task DeleteUser(User user);
        Task<bool> CheckUserPassword(string login, string password);
        Task ChangeUserRole(int userId, int roleId);
        Task<int> AddRole(UserRole role);
        Task DeleteRole(UserRole role);
        Task UpdateRole(UserRole role);
        Task<bool> CheckLogin(string login);
        Task<User> GetUserByLogin(string login);
        Task<User> GetUserWithOrder(int id);
        Task<int> UsersCount();
        Task<IEnumerable<User>> GetUsers(int skip, int take);
    }
}
