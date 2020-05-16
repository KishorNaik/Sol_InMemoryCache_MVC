using Sol_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sol_Demo.ViewModels
{
    public class UserViewModel
    {
        public UserModel Users { get; set; }

        public List<UserModel> UserList { get; set; }
    }
}
