using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.ContactInfo;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// ContactInfo Restful WebAPI For Publish
    /// </summary>
    [Route("api/[controller]")]
    [ValidRequest]
    [ApiController]
    public class ContactInfoPublishController : ControllerBase
    {
        private readonly IPublishCommand _publishCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishCommand"></param>
        public ContactInfoPublishController(IPublishCommand publishCommand)
        {
            _publishCommand = publishCommand;
        }

        /// <summary>
        /// ContactInfo - 發佈資料
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
            return _publishCommand.Publish(objInsert);
        }
    }
}
