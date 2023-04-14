using Microsoft.AspNetCore.Mvc;

namespace RedLock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: request.");
            string orderNo = null;

            var resource = $"OrderNo:{DateTime.Now:yyyyMMddHHmmssfff}"; //resource lock key
            var expiry = TimeSpan.FromSeconds(60);                      //lock key expire 時間
            do
            {
                // blocks 直到取得 lock 資源
                using (var redLock = RedisConnectionFactory.RedisLockFactory.CreateLockAsync(resource, expiry).Result)
                {
                    // 確定取得 lock 所有權
                    if (redLock.IsAcquired)
                    {
                        _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: lock start.");

                        // 執行需要獨佔資源的核心工作
                        orderNo = resource;

                        _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: lock end.");
                    }
                    else
                    {
                        _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: not get the locker.");

                        SpinWait.SpinUntil(() => false, TimeSpan.FromMilliseconds(1));
                        resource = $"OrderNo:{DateTime.Now:yyyyMMddHHmmssfff}";
                    }
                }
            } while (orderNo == null);

            _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: response.");
            return orderNo;
        }
    }
}
