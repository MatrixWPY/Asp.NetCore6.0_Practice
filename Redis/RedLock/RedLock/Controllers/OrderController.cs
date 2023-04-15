using Microsoft.AspNetCore.Mvc;
using RedLock.Services.Interface;

namespace RedLock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRedisService _redisService;

        public OrderController(ILogger<OrderController> logger, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }

        [HttpGet]
        public string Get()
        {
            string orderNo = null;

            var resource = $"OrderNo";                  //resource lock key
            var expiry = TimeSpan.FromSeconds(10);      //lock key expire 時間
            var wait = TimeSpan.FromSeconds(10);        //放棄重試時間
            var retry = TimeSpan.FromMilliseconds(1);   //重試間隔時間
            do
            {
                // blocks 直到取得 lock 資源或是達到放棄重試時間
                using (var redLock = _redisService.RedisLockFactory.CreateLockAsync(resource, expiry, wait, retry).Result)
                {
                    // 確定取得 lock 所有權
                    if (redLock.IsAcquired)
                    {
                        // 執行需要獨佔資源的核心工作
                        var newOrderNo = $"{DateTime.Now:yyyyMMddHHmmssfff}";
                        var redisKey = "LastOrderNo";
                        if (_redisService.Exist(redisKey))
                        {
                            var lastOrderNo = _redisService.Get<string>(redisKey);
                            if (lastOrderNo == newOrderNo)
                            {
                                _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: OrderNo duplicate => Retry");
                            }
                            else
                            {
                                _redisService.Set(redisKey, newOrderNo, TimeSpan.FromSeconds(30));
                                orderNo = newOrderNo;
                            }
                        }
                        else
                        {
                            _redisService.Set(redisKey, newOrderNo, TimeSpan.FromSeconds(30));
                            orderNo = newOrderNo;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: Not get the locker => Retry");
                    }
                }
                // 脫離 using 範圍自動就會解除 lock
            } while (orderNo == null);

            _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: {orderNo}");
            return orderNo;
        }
    }
}
