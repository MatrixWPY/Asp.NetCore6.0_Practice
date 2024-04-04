using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.Filters;
using WebApi.Models.Data;
using WebApi.Models.Request;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// ContactInfo Restful WebAPI For Queue
    /// </summary>
    [Route("api/[controller]")]
    [ValidRequest]
    [ApiController]
    public class ContactInfoQueueController : ControllerBase
    {
        private readonly IQueueCommand _queueCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueCommand"></param>
        public ContactInfoQueueController(IQueueCommand queueCommand)
        {
            _queueCommand = queueCommand;
        }

        /// <summary>
        /// ContactInfo - 接收資料從 Queue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultRP<IEnumerable<ContactInfo>> Get(int cnt)
        {
            return _queueCommand.Receive<ContactInfo>(cnt);
        }

        /// <summary>
        /// ContactInfo - 傳送資料至 Queue
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultRP<bool> Post([FromBody] ContactInfoAddRQ objRQ)
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
            return _queueCommand.Send(objInsert);
        }
    }
}
