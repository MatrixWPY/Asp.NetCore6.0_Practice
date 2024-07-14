using WebApiFactory.Commands.Interface;
using WebApiFactory.Models.Data;
using WebApiFactory.Models.Request;
using WebApiFactory.Models.Response;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoRedisStringCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IContactInfoService _contactInfoService;
        private readonly IRedisService _redisService;
        private const string _redisQueryByID = $"{nameof(ContactInfo)}:{nameof(QueryByID)}";
        private const string _redisQueryByCondition = $"{nameof(ContactInfo)}:{nameof(QueryByCondition)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactInfoService"></param>
        /// <param name="redisService"></param>
        public ContactInfoRedisStringCommand(IContactInfoService contactInfoService, IRedisService redisService)
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
            var redisKey = $"{_redisQueryByID}:{id}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.GetObject<ContactInfo>(redisKey);
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
                    _redisService.SetObjectAsync(redisKey, res, TimeSpan.FromMinutes(5));
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

            var redisKey = $"{_redisQueryByCondition}:{string.Join('&', dicParams.Select(e => e.Key + "=" + e.Value?.ToString()))}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.GetObject<(int totalCnt, IEnumerable<ContactInfo> data)>(redisKey);
                return SuccessRP(new PageDataRP<IEnumerable<ContactInfo>>()
                {
                    PageInfo = new PageInfoRP()
                    {
                        CurrentIndex = objRQ.PageIndex,
                        CurrentSize = res.data.Count(),
                        PageCnt = res.totalCnt % objRQ.PageSize == 0 ? res.totalCnt / objRQ.PageSize : res.totalCnt / objRQ.PageSize + 1,
                        TotalCnt = res.totalCnt
                    },
                    Data = res.data
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
                    _redisService.SetObjectAsync(redisKey, res, TimeSpan.FromMinutes(5));
                    return SuccessRP(new PageDataRP<IEnumerable<ContactInfo>>()
                    {
                        PageInfo = new PageInfoRP()
                        {
                            CurrentIndex = objRQ.PageIndex,
                            CurrentSize = res.data.Count(),
                            PageCnt = res.totalCnt % objRQ.PageSize == 0 ? res.totalCnt / objRQ.PageSize : res.totalCnt / objRQ.PageSize + 1,
                            TotalCnt = res.totalCnt
                        },
                        Data = res.data
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
                var objCache = _contactInfoService.Query(objInsert.ContactInfoID);
                _redisService.SetObjectAsync($"{_redisQueryByID}:{objCache.ContactInfoID}", objCache, TimeSpan.FromMinutes(5));
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");
                return SuccessRP(objCache);
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
                var objCache = _contactInfoService.Query(objUpdate.ContactInfoID);
                _redisService.SetObjectAsync($"{_redisQueryByID}:{objCache.ContactInfoID}", objCache, TimeSpan.FromMinutes(5));
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");
                return SuccessRP(objCache);
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
                var objCache = _contactInfoService.Query(objUpdate.ContactInfoID);
                _redisService.SetObjectAsync($"{_redisQueryByID}:{objCache.ContactInfoID}", objCache, TimeSpan.FromMinutes(5));
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");
                return SuccessRP(objCache);
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
                _redisService.RemoveAsync(liID.Select(e => $"{_redisQueryByID}:{e}"));
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");
                return SuccessRP(res);
            }
            else
            {
                return FailRP<bool>(4, "Delete Fail");
            }
        }
    }
}
