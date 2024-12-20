﻿using WebApi.Commands.Interface;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscribeRedisListCommand : ISubscribeCommand
    {
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisService"></param>
        public SubscribeRedisListCommand(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public void Subscribe<T>(Func<T, bool> func)
        {
            _redisService.SubscribeListQueue("Channel_ListQueue", func);
        }
    }
}
