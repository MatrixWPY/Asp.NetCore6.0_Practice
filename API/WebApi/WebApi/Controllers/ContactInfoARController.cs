using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.ContactInfo;
using WebApi.Filters;

namespace WebApi.Controllers
{
    /// <summary>
    /// ContactInfo AttributeRoute WebAPI
    /// </summary>
    [Route("V1")]
    [ValidRequest]
    [ApiController]
    public class ContactInfoARController : ControllerBase
    {
        private readonly IContactInfoCommand _contactInfoCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoCommand"></param>
        public ContactInfoARController(IContactInfoCommand contactInfoCommand)
        {
            _contactInfoCommand = contactInfoCommand;
        }

        /// <summary>
        /// ContactInfo - 單筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C101")]
        public ApiResultRP<QueryRP> GetContactInfo(IdRQ objRQ)
        {
            return _contactInfoCommand.QueryByID(objRQ.ID ?? 0);
        }

        /// <summary>
        /// ContactInfo - 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C102")]
        public ApiResultRP<PageDataRP<IEnumerable<QueryRP>>> ListContactInfo(QueryRQ objRQ)
        {
            return _contactInfoCommand.QueryByCondition(objRQ);
        }

        /// <summary>
        /// ContactInfo - 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C103")]
        public ApiResultRP<QueryRP> CreateContactInfo(CreateRQ objRQ)
        {
            return _contactInfoCommand.Create(objRQ);
        }

        /// <summary>
        /// ContactInfo - 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C104")]
        public ApiResultRP<QueryRP> EditContactInfo(EditRQ objRQ)
        {
            return _contactInfoCommand.Edit(objRQ);
        }

        /// <summary>
        /// ContactInfo - 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C105")]
        public ApiResultRP<QueryRP> EditPartialContactInfo(EditPartialRQ objRQ)
        {
            return _contactInfoCommand.EditPartial(objRQ);
        }

        /// <summary>
        /// ContactInfo - 刪除資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C106")]
        public ApiResultRP<bool> RemoveContactInfo(IdRQ objRQ)
        {
            return _contactInfoCommand.Remove(new List<long>() { objRQ.ID ?? 0 });
        }
    }
}
