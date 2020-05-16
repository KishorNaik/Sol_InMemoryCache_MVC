using Sol_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sol_Demo.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetUserDataAsync();
    }

    public class UserRepository : IUserRepository
    {
        public Task<IEnumerable<UserModel>> GetUserDataAsync()
        {
            var userListData = new List<UserModel>();
            userListData.Add(new UserModel()
            {
                Id = 1,
                FirstName = "Kishor",
                LastName = "Naik"
            });
            userListData.Add(new UserModel()
            {
                Id = 2,
                FirstName = "Eshaan",
                LastName = "Naik"
            });

            return Task.FromResult<IEnumerable<UserModel>>(userListData);
        }
    }
}
