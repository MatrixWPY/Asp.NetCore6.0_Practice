using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApiFactory.DtoModels.Common;
using WebApiFactory.DtoModels.ContactInfo;
using WebApiFactory.Factories.Interface;
using WebApiFactory.Filters;

namespace WebApiFactory.Controllers
{
    /// <summary>
    /// ContactInfo Restful WebAPI
    /// </summary>
    [Route("api/[controller]")]
    [ValidRequest]
    [ApiController]
    public class ContactInfoRestfulController : ControllerBase
    {
        private readonly ICommandFactory _commandFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoCommand"></param>
        public ContactInfoRestfulController(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        /// <summary>
        /// ContactInfo - 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commandType"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public ApiResultRP<QueryRP> Get(
            [FromRoute] long id,
            [FromQuery][Required] string commandType, [FromQuery][Required] string serviceType
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(commandType);
            return command.QueryByID(id, serviceType);
        }

        /// <summary>
        /// ContactInfo - 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultRP<PageDataRP<IEnumerable<QueryRP>>> Get(
            [FromQuery] QueryRQ objRQ
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(objRQ.CommandType);
            return command.QueryByCondition(objRQ);
        }

        /// <summary>
        /// ContactInfo - 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultRP<QueryRP> Post(
            [FromBody] CreateRQ objRQ
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(objRQ.CommandType);
            return command.Create(objRQ);
        }

        /// <summary>
        /// ContactInfo - 修改資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        public ApiResultRP<QueryRP> Put(
            [FromRoute] long id,
            [FromBody] RestfulEditRQ objRQ
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(objRQ.CommandType);
            return command.Edit(new EditRQ()
            {
                CommandType = objRQ.CommandType,
                ServiceType = objRQ.ServiceType,
                ID = id,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            });
        }

        /// <summary>
        /// ContactInfo - 部分修改資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPatch, Route("{id}")]
        public ApiResultRP<QueryRP> Patch(
            [FromRoute] long id,
            [FromBody] RestfulEditPartialRQ objRQ
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(objRQ.CommandType);
            return command.EditPartial(new EditPartialRQ()
            {
                CommandType = objRQ.CommandType,
                ServiceType = objRQ.ServiceType,
                ID = id,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            });
        }

        /// <summary>
        /// ContactInfo - 刪除資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commandType"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        public ApiResultRP<bool> Delete(
            [FromRoute] long id,
            [FromQuery][Required] string commandType, [FromQuery][Required] string serviceType
        )
        {
            var command = _commandFactory.CreateContactInfoCommand(commandType);
            return command.Remove(new List<long>() { id }, serviceType);
        }
    }
}
