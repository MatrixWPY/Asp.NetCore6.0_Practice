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
    public class ContactInfoRedisCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IContactInfoService _contactInfoService;
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoService"></param>
        /// <param name="redisService"></param>
        public ContactInfoRedisCommand(IContactInfoService contactInfoService, IRedisService redisService)
        {
            _contactInfoService = contactInfoService;
            _redisService = redisService;
        }

        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApiResultRP<ContactInfo> QueryByID(long id)
        {
            var redisKey = $"{nameof(ContactInfo)}:{nameof(QueryByID)}:{id}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.Get<ContactInfo>(redisKey);
                return SuccessRP(res);
            }
            else
            {
                var res = _contactInfoService.Query(id);
                if (res == null)
                {
                    return FailRP<ContactInfo>(1, "No Data");
                }
                else
                {
                    _redisService.SetAsync(redisKey, res, TimeSpan.FromMinutes(5));
                    return SuccessRP(res);
                }
            }
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

            var redisKey = $"{nameof(ContactInfo)}:{nameof(QueryByCondition)}:{string.Join(',', dicParams.Select(e => e.Value?.ToString()))}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.Get<(int, IEnumerable<ContactInfo>)>(redisKey);
                return SuccessRP(new PageDataRP<IEnumerable<ContactInfo>>()
                {
                    PageInfo = new PageInfoRP()
                    {
                        PageIndex = objRQ.PageIndex,
                        PageSize = res.Item2.Count(),
                        PageCnt = (res.Item1 % objRQ.PageSize == 0 ? res.Item1 / objRQ.PageSize : res.Item1 / objRQ.PageSize + 1),
                        TotalCnt = res.Item1
                    },
                    Data = res.Item2
                });
            }
            else
            {
                var res = _contactInfoService.Query(dicParams);
                if (res.Item2 == null)
                {
                    return FailRP<PageDataRP<IEnumerable<ContactInfo>>>(1, "No Data");
                }
                else
                {
                    _redisService.SetAsync(redisKey, res, TimeSpan.FromMinutes(5));
                    return SuccessRP(new PageDataRP<IEnumerable<ContactInfo>>()
                    {
                        PageInfo = new PageInfoRP()
                        {
                            PageIndex = objRQ.PageIndex,
                            PageSize = res.Item2.Count(),
                            PageCnt = (res.Item1 % objRQ.PageSize == 0 ? res.Item1 / objRQ.PageSize : res.Item1 / objRQ.PageSize + 1),
                            TotalCnt = res.Item1
                        },
                        Data = res.Item2
                    });
                }
            }
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

            var res = _contactInfoService.Insert(objInsert);
            if (res)
            {
                _redisService.RemoveByKeyAsync($"{nameof(ContactInfo)}");
                return SuccessRP(_contactInfoService.Query(objInsert.ContactInfoID));
            }
            else
            {
                return FailRP<ContactInfo>(2, "Add Fail");
            }
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

            var res = _contactInfoService.Update(objUpdate);
            if (res)
            {
                _redisService.RemoveByKeyAsync($"{nameof(ContactInfo)}");
                return SuccessRP(_contactInfoService.Query(objUpdate.ContactInfoID));
            }
            else
            {
                return FailRP<ContactInfo>(3, "Edit Fail");
            }
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

            var res = _contactInfoService.Update(objUpdate);
            if (res)
            {
                _redisService.RemoveByKeyAsync($"{nameof(ContactInfo)}");
                return SuccessRP(_contactInfoService.Query(objUpdate.ContactInfoID));
            }
            else
            {
                return FailRP<ContactInfo>(3, "Edit Partial Fail");
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="liID"></param>
        /// <returns></returns>
        public ApiResultRP<bool> DeleteByID(IEnumerable<long> liID)
        {
            var res = _contactInfoService.Delete(liID);
            if (res)
            {
                _redisService.RemoveByKeyAsync($"{nameof(ContactInfo)}");
                return SuccessRP(res);
            }
            else
            {
                return FailRP<bool>(4, "Delete Fail");
            }
        }
    }
}
