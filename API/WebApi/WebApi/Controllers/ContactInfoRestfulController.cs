using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.Filters;
using WebApi.Models.Request;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// ContactInfo Restful WebAPI
    /// </summary>
    [Route("api/[controller]")]
    [ValidRequest]
    [ApiController]
    public class ContactInfoRestfulController : ControllerBase
    {
        private readonly IContactInfoCommand _contactInfoCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoCommand"></param>
        public ContactInfoRestfulController(IContactInfoCommand contactInfoCommand)
        {
            _contactInfoCommand = contactInfoCommand;
        }

        /// <summary>
        /// ContactInfo - 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public ApiResultRP<QueryRP> Get([FromRoute] long id)
        {
            return _contactInfoCommand.QueryByID(id);
        }

        /// <summary>
        /// ContactInfo - 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultRP<PageDataRP<IEnumerable<QueryRP>>> Get([FromQuery] QueryRQ objRQ)
        {
            return _contactInfoCommand.QueryByCondition(objRQ);
        }

        /// <summary>
        /// ContactInfo - 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultRP<QueryRP> Post([FromBody] CreateRQ objRQ)
        {
            return _contactInfoCommand.Create(objRQ);
        }

        /// <summary>
        /// ContactInfo - 修改資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        public ApiResultRP<QueryRP> Put([FromRoute] long id, [FromBody] RestfulEditRQ objRQ)
        {
            return _contactInfoCommand.Edit(new EditRQ()
            {
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
        public ApiResultRP<QueryRP> Patch([FromRoute] long id, [FromBody] RestfulEditPartialRQ objRQ)
        {
            return _contactInfoCommand.EditPartial(new EditPartialRQ()
            {
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
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        public ApiResultRP<bool> Delete([FromRoute] long id)
        {
            return _contactInfoCommand.Remove(new List<long>() { id });
        }
    }
}
