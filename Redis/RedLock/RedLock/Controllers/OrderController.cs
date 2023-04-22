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
        private readonly IRedlockService _redlockService;

        public OrderController(ILogger<OrderController> logger, IRedisService redisService, IRedlockService redlockService)
        {
            _logger = logger;
            _redisService = redisService;
            _redlockService = redlockService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string orderNo = null;

            var resource = $"OrderNo";                  //resource lock key
            var expiry = TimeSpan.FromSeconds(10);      //lock key expire 時間
            var wait = TimeSpan.FromSeconds(10);        //放棄重試時間
            var retry = TimeSpan.FromMilliseconds(1);   //重試間隔時間
            do
            {
                orderNo = await _redlockService.AcquireLockAsync(resource, expiry, wait, retry,
                        () =>
                        {
                            var newOrderNo = $"{DateTime.Now:yyyyMMddHHmmssfff}";
                            var redisKey = "LastOrderNo";
                            if (_redisService.Exist(redisKey))
                            {
                                var lastOrderNo = _redisService.Get<string>(redisKey);
                                if (lastOrderNo == newOrderNo)
                                {
                                    _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: OrderNo duplicate => Retry");
                                    return null;
                                }
                                else
                                {
                                    _redisService.Set(redisKey, newOrderNo, TimeSpan.FromSeconds(30));
                                    return newOrderNo;
                                }
                            }
                            else
                            {
                                _redisService.Set(redisKey, newOrderNo, TimeSpan.FromSeconds(30));
                                return newOrderNo;
                            }
                        },
                        () =>
                        {
                            _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: Not get the locker => Retry");
                            return null;
                        }
                );
            } while (orderNo == null);

            _logger.LogInformation($"{Thread.CurrentThread.ManagedThreadId}: {orderNo}");
            return orderNo;
        }
    }
}
