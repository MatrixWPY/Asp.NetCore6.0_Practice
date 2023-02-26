﻿using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.Filters;
using WebApi.Models.Data;
using WebApi.Models.Request;
using WebApi.Models.Response;

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
        public ApiResultRP<ContactInfo> GetContactInfo(IdRQ objRQ)
        {
            return _contactInfoCommand.QueryByID(objRQ.ID ?? 0);
        }

        /// <summary>
        /// ContactInfo - 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C102")]
        public ApiResultRP<PageDataRP<IEnumerable<ContactInfo>>> ListContactInfo(ContactInfoQueryRQ objRQ)
        {
            return _contactInfoCommand.QueryByCondition(objRQ);
        }

        /// <summary>
        /// ContactInfo - 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C103")]
        public ApiResultRP<ContactInfo> AddContactInfo(ContactInfoAddRQ objRQ)
        {
            return _contactInfoCommand.Add(objRQ);
        }

        /// <summary>
        /// ContactInfo - 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C104")]
        public ApiResultRP<ContactInfo> EditContactInfo(ContactInfoEditRQ objRQ)
        {
            return _contactInfoCommand.Edit(objRQ);
        }

        /// <summary>
        /// ContactInfo - 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C105")]
        public ApiResultRP<ContactInfo> EditPartialContactInfo(ContactInfoEditPartialRQ objRQ)
        {
            return _contactInfoCommand.EditPartial(objRQ);
        }

        /// <summary>
        /// ContactInfo - 刪除資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("C106")]
        public ApiResultRP<bool> DeleteContactInfo(IdRQ objRQ)
        {
            return _contactInfoCommand.DeleteByID(new List<long>() { objRQ.ID ?? 0 });
        }
    }
}
