using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RedisDemo.InMemory.Models;

namespace RedisDemo.InMemory.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            //We can check whether the data we want to add to cache is exist.
            //First option
            if (String.IsNullOrEmpty(_memoryCache.Get<string>("TimeNow")))
            {
                //New data added to inMemory cache service by using Set() method.
                //_memoryCache.Set("TimeNow",DateTime.Now);
                _memoryCache.Set<string>("TimeNow", DateTime.Now.ToString());
            }

            //We can use TryGetValue() method to check whether data is in the cache.
            //Take care about second parameter starts "out"
            //If cache is exist then we can use it thanks to out keyword.
            //Second option
            if (!_memoryCache.TryGetValue("TimeNow", out string dateNowCache))
            {
                _memoryCache.Set<string>("TimeNow", DateTime.Now.ToString());
            }

            //We can add complex data types into in-memory cache.
            //In-memory caching mechanism serializes it automatically.
            Product product = new Product() { Id=1,Name="Pencil", Price=3};
            _memoryCache.Set<Product>("Product1", product);
            
            return View("Index",dateNowCache);
        }

        public IActionResult Show()
        {
            //If want to ensure existence of data we should use GetOrCreate() method.
            //If the data is exist, it returns the value. Otherwise we can create new one
            //and keep it in in-memory cache.
            _memoryCache.GetOrCreate<string>("TimeNow", entry =>
            {
                return DateTime.Now.ToString();
            });

            //The data in the in-memory cache has called and taken by using Get() method.
            //ViewBag.TimeNow = _memoryCache.Get("TimeNow");
            ViewBag.TimeNow = _memoryCache.Get<string>("TimeNow");

            //We can remove data in the in-memory cache by using Remove() method.
            _memoryCache.Remove("TimeNow");

            ViewBag.Product = _memoryCache.Get<Product>("Product1"); 
            return View();
        }
    }
}
