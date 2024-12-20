﻿using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueRabbitMQCommand : BaseCommand, IQueueCommand
    {
        private readonly IRabbitMQService _rabbitMQService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rabbitMQService"></param>
        public QueueRabbitMQCommand(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public ApiResultRP<IEnumerable<T>> Receive<T>(int cnt)
        {
            List<T> res = new List<T>();

            _rabbitMQService.ReceiveDirectWithBlock<T>(typeof(T).Name, cnt, 3,
                (obj) =>
                {
                    res.Add(obj);
                    return true;
                }
            );

            return SuccessRP(res.AsEnumerable());
        }

        public ApiResultRP<bool> Send<T>(T value)
        {
            _rabbitMQService.SendDirect(nameof(QueueRabbitMQCommand), nameof(Send), typeof(T).Name, value);
            return SuccessRP(true);
        }
    }
}
