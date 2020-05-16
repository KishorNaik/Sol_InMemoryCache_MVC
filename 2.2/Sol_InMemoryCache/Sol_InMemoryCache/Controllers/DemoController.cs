using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Sol_InMemoryCache.Controllers
{
    public class DemoController : Controller
    {

        private readonly IMemoryCache memoryCache = null;
        private readonly MemoryCacheEntryOptions options = null;

        public DemoController(IMemoryCache memoryCache,
            MemoryCacheEntryOptions option
            )
        {
            this.memoryCache = memoryCache;
            this.options = option;
        }

        #region Static Function
        private static void MyCallBack(object key, object value,EvictionReason reason, object state)
        {
            var message = $"Cache entry was removed : {reason}";
            ((DemoController)state)
                ?.memoryCache
                ?.Set<string>("callBackMessage", message);
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            string timeStamp = null;

            if(!memoryCache.TryGetValue<string>("timeStamp",out timeStamp))
            {
                options.Priority = CacheItemPriority.Normal;
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(5);
                options.SlidingExpiration = TimeSpan.FromSeconds(5);
                options.RegisterPostEvictionCallback(MyCallBack, this);

                timeStamp =
                memoryCache
                ?.Set<String>("timeStamp", DateTime.Now.ToString(),options);
            }
            
            ViewBag.TimeStamp = timeStamp;

            return View();
        }

        public IActionResult Show()
        {

            var timeStampShow =
                    memoryCache
                    ?.Get<String>("timeStamp");

            ViewBag.TimeStampShow = timeStampShow;

            var timeStampExpire =
                memoryCache
                ?.Get<String>("callBackMessage");

            ViewBag.TimeStampExpire = timeStampExpire;

            return View();
        }

        #endregion 
    }
}