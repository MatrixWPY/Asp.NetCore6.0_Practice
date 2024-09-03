using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.ContactInfo;
using WebApi.Filters;
using WebApi.Models;

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
        public ApiResultRP<IEnumerable<QueryRP>> Get(int cnt)
        {
            var res = _queueCommand.Receive<ContactInfo>(cnt);
            return new ApiResultRP<IEnumerable<QueryRP>>
            {
                Code = res.Code,
                Msg = res.Msg,
                Result = res.Result.Select(e => new QueryRP
                {
                    ContactInfoID = e.ContactInfoID,
                    Name = e.Name,
                    Nickname = e.Nickname,
                    Gender = (short)e.Gender,
                    Age = e.Age,
                    PhoneNo = e.PhoneNo,
                    Address = e.Address
                })
            };
        }

        /// <summary>
        /// ContactInfo - 傳送資料至 Queue
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultRP<bool> Post([FromBody] CreateRQ objRQ)
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
