using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace RedisDemo.InMemory.Controllers
{
    public class UserController : Controller
    {
        private IMemoryCache _memoryCache;
        public UserController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            // ###---MemoryCacheEntryOptions---###

            /*
                AbsoluteExpiration : The data in cache is removed after time over once.
                
                SlidingExpiration  : The data in cache is removed after time over once.
                however if the data in cache call before time over, the expiration duration 
                is refreshed each call.

                If we use SlidingExpiration and we call data in cache too much, the data
                might get old. Because they need to be updated. For this reason we may use
                both AbsoluteExpiration and SlidingExpiration together for best practise.

                In the following codes, we can call "TimeNow" cache data before its 
                SlidingExpiration expired many times but when AbsoluteExpiration
                expired data is removed from cache eventually.
            */

            /*
                The datas in cache is kept in limited storage for sure. For this reason
                we should handle the datas in cache as possible as effectively.
                In In-memory cache, there is Priority property. This property provides
                4 level prioritory(Low -> Normal -> High -> NeverRemove).
                If new datas gonna add to cache and caching storage is full, the mechanism 
                starts removing datas from least priority level cache data.
            */
            
            /*
                RegisterPostEvictionCallback() method give informations about the data removed
                from cache. (Key name, Key value, Removing reason, state)
            */


            if (!_memoryCache.TryGetValue("TimeNow", out string cacheDateNow))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(30);
                options.SlidingExpiration = TimeSpan.FromSeconds(10);
                options.Priority = CacheItemPriority.High;
                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _memoryCache.Set("Callback", $"Key: {key}, Value: {value} pair removed." +
                        $" Reason: {reason}, State: {state}");
                });
                _memoryCache.Set<string>("TimeNow", DateTime.Now.ToString(), options);
            }
            return View("Index", cacheDateNow);
        }

        public IActionResult Details()
        {
            ViewBag.TimeNow = _memoryCache.Get<string>("TimeNow");
            ViewBag.Callback = _memoryCache.Get<string>("Callback");

            /*
             * Different way of usage of "MemoryCacheEntryOptions"
              
                ViewBag.TimeNow = _memoryCache.GetOrCreate<string>("TimeNow", entry =>
                {
                    MemoryCacheEntryOptions options = new();
                    options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
                    entry.SetOptions(options);
                    entry.Value = DateTime.Now.ToString();
                    return DateTime.Now.ToString();
                });
            */

            return View();
        }
    }
}
