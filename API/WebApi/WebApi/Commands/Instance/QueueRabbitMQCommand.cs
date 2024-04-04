using WebApi.Commands.Interface;
using WebApi.Models.Data;
using WebApi.Models.Request;
using WebApi.Models.Response;
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

        public ApiResultRP<List<ContactInfo>> Receive(int cnt)
        {
            List<ContactInfo> res = new List<ContactInfo>();

            _rabbitMQService.ReceiveDirect<ContactInfo>("ContactInfo", cnt, 3, (obj) =>
            {
                res.Add(obj);
                return true;
            });

            return SuccessRP(res);
        }

        public ApiResultRP<bool> Send(ContactInfoAddRQ objRQ)
        {
            var objInsert = new ContactInfo()
            {
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            };
            _rabbitMQService.SendDirect("QueueCommand", "Send", "ContactInfo", objInsert);
            return SuccessRP(true);
        }
    }
}
