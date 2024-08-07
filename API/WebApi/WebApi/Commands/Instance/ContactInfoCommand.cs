﻿using WebApi.Commands.Interface;
using WebApi.Models.Data;
using WebApi.Models.Request;
using WebApi.Models.Response;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IContactInfoService _contactInfoService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoService"></param>
        public ContactInfoCommand(IContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApiResultRP<ContactInfo> QueryByID(long id)
        {
            var res = _contactInfoService.Query(id);
            return res == null ? FailRP<ContactInfo>(1, "No Data") : SuccessRP(res);
        }

        /// <summary>
        /// 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<PageDataRP<IEnumerable<ContactInfo>>> QueryByCondition(ContactInfoQueryRQ objRQ)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(objRQ.Name) == false)
            {
                dicParams["Name"] = objRQ.Name;
            }
            if (string.IsNullOrWhiteSpace(objRQ.Nickname) == false)
            {
                dicParams["Nickname"] = $"%{objRQ.Nickname}%";
            }
            if (objRQ.Gender.HasValue)
            {
                dicParams["Gender"] = objRQ.Gender;
            }
            dicParams["RowStart"] = (objRQ.PageIndex - 1) * objRQ.PageSize;
            dicParams["RowLength"] = objRQ.PageSize;

            var res = _contactInfoService.Query(dicParams);
            return res.Item2 == null ? FailRP<PageDataRP<IEnumerable<ContactInfo>>>(1, "No Data")
                                     : SuccessRP(new PageDataRP<IEnumerable<ContactInfo>>()
                                     {
                                         PageInfo = new PageInfoRP()
                                         {
                                             CurrentIndex = objRQ.PageIndex,
                                             CurrentSize = res.data.Count(),
                                             PageCnt = (res.totalCnt % objRQ.PageSize == 0 ? res.totalCnt / objRQ.PageSize : res.totalCnt / objRQ.PageSize + 1),
                                             TotalCnt = res.totalCnt
                                         },
                                         Data = res.data
                                     });
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<ContactInfo> Add(ContactInfoAddRQ objRQ)
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
            return _contactInfoService.Insert(objInsert) == false ? FailRP<ContactInfo>(2, "Add Fail")
                                                                  : SuccessRP(_contactInfoService.Query(objInsert.ContactInfoID));
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<ContactInfo> Edit(ContactInfoEditRQ objRQ)
        {
            var objOrigin = _contactInfoService.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<ContactInfo>(1, "No Data");
            }

            var objUpdate = new ContactInfo()
            {
                ContactInfoID = objRQ.ID ?? 0,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            };
            return _contactInfoService.Update(objUpdate) == false ? FailRP<ContactInfo>(3, "Edit Fail")
                                                                  : SuccessRP(_contactInfoService.Query(objUpdate.ContactInfoID));
        }

        /// <summary>
        /// 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<ContactInfo> EditPartial(ContactInfoEditPartialRQ objRQ)
        {
            var objOrigin = _contactInfoService.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<ContactInfo>(1, "No Data");
            }

            var objUpdate = new ContactInfo()
            {
                ContactInfoID = objRQ.ID ?? 0,
                Name = string.IsNullOrWhiteSpace(objRQ.Name) ? objOrigin.Name : objRQ.Name,
                Nickname = string.IsNullOrWhiteSpace(objRQ.Nickname) ? objOrigin.Nickname : objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender ?? objOrigin.Gender,
                Age = objRQ.Age ?? objOrigin.Age,
                PhoneNo = string.IsNullOrWhiteSpace(objRQ.PhoneNo) ? objOrigin.PhoneNo : objRQ.PhoneNo,
                Address = string.IsNullOrWhiteSpace(objRQ.Address) ? objOrigin.Address : objRQ.Address
            };
            return _contactInfoService.Update(objUpdate) == false ? FailRP<ContactInfo>(3, "Edit Partial Fail")
                                                                  : SuccessRP(_contactInfoService.Query(objUpdate.ContactInfoID));
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="liID"></param>
        /// <returns></returns>
        public ApiResultRP<bool> DeleteByID(IEnumerable<long> liID)
        {
            var res = _contactInfoService.Delete(liID);
            return res == false ? FailRP<bool>(4, "Delete Fail") : SuccessRP(res);
        }
    }
}
