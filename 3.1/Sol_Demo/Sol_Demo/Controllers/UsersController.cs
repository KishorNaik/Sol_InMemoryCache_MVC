using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Sol_Demo.Models;
using Sol_Demo.Repository;
using Sol_Demo.ViewModels;

namespace Sol_Demo.Controllers
{
    public class UsersController : Controller
    {
        private readonly IMemoryCache memoryCache = null;
        private readonly IUserRepository userRepository = null;

        public UsersController(IMemoryCache memoryCache,IUserRepository userRepository)
        {
            this.memoryCache = memoryCache;
            this.userRepository = userRepository;

            UserVM = new UserViewModel();
        }

        #region Public Property
        [BindProperty]
        public UserViewModel UserVM { get; set; }

        [BindProperty(SupportsGet =true)]
        public int id { get; set; }
        #endregion 

        #region Private Method
        private async Task BindUserGridAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<UserModel> cacheUserList = await CacheUserListAsync();

            //UserVM.UserList = (await userRepository.GetUserDataAsync()).ToList();
            UserVM.UserList = cacheUserList;

            stopwatch.Stop();
            ViewBag.TotalLoadTime = stopwatch.Elapsed.ToString();
        }

        private async Task<List<UserModel>> CacheUserListAsync()
        {
            List<UserModel> cacheUserList = null;

            bool isCache = this.memoryCache.TryGetValue<List<UserModel>>("UserListCacheKey", out cacheUserList);

            if (isCache == false)
            {
                cacheUserList =
                    await this.memoryCache.GetOrCreateAsync<List<UserModel>>("UserListCacheKey", async (cacheEntry) =>
                    {
                        return (await userRepository.GetUserDataAsync()).ToList();

                    });
            }

            return cacheUserList;
        }

        private Task EditAsync()
        {
            var cacheUserList = this.memoryCache.Get<List<UserModel>>("UserListCacheKey");

            var filterUser =
                    cacheUserList
                    .FirstOrDefault((leUserModel) => leUserModel.Id == id);

            UserVM.Users = filterUser;

            return Task.CompletedTask;
        }

        private Task UpdateAsync()
        {
            var cacheUserList = this.memoryCache.Get<List<UserModel>>("UserListCacheKey");

            var oldUserObject =
                    cacheUserList
                    ?.FirstOrDefault((leUserModel) => leUserModel.Id == UserVM.Users.Id);

            oldUserObject.FirstName = UserVM.Users.FirstName;
            oldUserObject.LastName = UserVM.Users.LastName;

            // Remove Cache Object
            this.memoryCache.Remove("UserListCacheKey");

            // ReSet Cache Object
            this.memoryCache.Set<List<UserModel>>("UserListCacheKey", cacheUserList);

            return Task.CompletedTask;
        }
        #endregion

        


        public async Task< IActionResult> Index()
        {
            await BindUserGridAsync();
            return View(UserVM);
        }

        public async Task<IActionResult> OnEdit()
        {
            await EditAsync();

            return View(UserVM);
        }

        public async Task<IActionResult> OnUpdate()
        {
            await UpdateAsync();

            return RedirectToAction("Index");
        }

    }
}