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
        public ApiResultRP<QueryRP> QueryByID(long id)
        {
            var res = _contactInfoService.Query(id);

            if (res == null)
            {
                return FailRP<QueryRP>(1, "No Data");
            }
            else
            {
                return SuccessRP(new QueryRP
                {
                    ContactInfoID = res.ContactInfoID,
                    Name = res.Name,
                    Nickname = res.Nickname,
                    Gender = (short?)res.Gender,
                    Age = res.Age,
                    PhoneNo = res.PhoneNo,
                    Address = res.Address
                });
            }
        }

        /// <summary>
        /// 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<PageDataRP<IEnumerable<QueryRP>>> QueryByCondition(QueryRQ objRQ)
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

            if (res.data == null)
            {
                return FailRP<PageDataRP<IEnumerable<QueryRP>>>(1, "No Data");
            }
            else
            {
                return SuccessRP(new PageDataRP<IEnumerable<QueryRP>>
                {
                    PageInfo = new PageInfoRP
                    {
                        CurrentIndex = objRQ.PageIndex,
                        CurrentSize = res.data.Count(),
                        PageCnt = (int)Math.Ceiling((decimal)res.totalCnt / objRQ.PageSize),
                        TotalCnt = res.totalCnt
                    },
                    Data = res.data.Select(e => new QueryRP
                    {
                        ContactInfoID = e.ContactInfoID,
                        Name = e.Name,
                        Nickname = e.Nickname,
                        Gender = (short?)e.Gender,
                        Age = e.Age,
                        PhoneNo = e.PhoneNo,
                        Address = e.Address
                    })
                });
            }
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> Create(CreateRQ objRQ)
        {
            var objInsert = new ContactInfo
            {
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            };

            var resInsert = _contactInfoService.Insert(objInsert);

            if (resInsert == false)
            {
                return FailRP<QueryRP>(2, "Create Fail");
            }
            else
            {
                var resQuery = _contactInfoService.Query(objInsert.ContactInfoID);

                return SuccessRP(new QueryRP
                {
                    ContactInfoID = resQuery.ContactInfoID,
                    Name = resQuery.Name,
                    Nickname = resQuery.Nickname,
                    Gender = (short?)resQuery.Gender,
                    Age = resQuery.Age,
                    PhoneNo = resQuery.PhoneNo,
                    Address = resQuery.Address
                });
            }
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> Edit(EditRQ objRQ)
        {
            var objOrigin = _contactInfoService.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
            }
            var objUpdate = new ContactInfo
            {
                ContactInfoID = objRQ.ID ?? 0,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            };

            var resUpdate = _contactInfoService.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Fail");
            }
            else
            {
                var resQuery = _contactInfoService.Query(objUpdate.ContactInfoID);

                return SuccessRP(new QueryRP
                {
                    ContactInfoID = resQuery.ContactInfoID,
                    Name = resQuery.Name,
                    Nickname = resQuery.Nickname,
                    Gender = (short?)resQuery.Gender,
                    Age = resQuery.Age,
                    PhoneNo = resQuery.PhoneNo,
                    Address = resQuery.Address
                });
            }
        }

        /// <summary>
        /// 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> EditPartial(EditPartialRQ objRQ)
        {
            var objOrigin = _contactInfoService.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
            }
            var objUpdate = new ContactInfo
            {
                ContactInfoID = objRQ.ID ?? 0,
                Name = string.IsNullOrWhiteSpace(objRQ.Name) ? objOrigin.Name : objRQ.Name,
                Nickname = string.IsNullOrWhiteSpace(objRQ.Nickname) ? objOrigin.Nickname : objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender ?? objOrigin.Gender,
                Age = objRQ.Age ?? objOrigin.Age,
                PhoneNo = string.IsNullOrWhiteSpace(objRQ.PhoneNo) ? objOrigin.PhoneNo : objRQ.PhoneNo,
                Address = string.IsNullOrWhiteSpace(objRQ.Address) ? objOrigin.Address : objRQ.Address
            };

            var resUpdate = _contactInfoService.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Partial Fail");
            }
            else
            {
                var resQuery = _contactInfoService.Query(objUpdate.ContactInfoID);

                return SuccessRP(new QueryRP
                {
                    ContactInfoID = resQuery.ContactInfoID,
                    Name = resQuery.Name,
                    Nickname = resQuery.Nickname,
                    Gender = (short?)resQuery.Gender,
                    Age = resQuery.Age,
                    PhoneNo = resQuery.PhoneNo,
                    Address = resQuery.Address
                });
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ApiResultRP<bool> Remove(IEnumerable<long> ids)
        {
            var res = _contactInfoService.Delete(ids);
            return res == false ? FailRP<bool>(4, "Remove Fail") : SuccessRP(res);
        }
    }
}
