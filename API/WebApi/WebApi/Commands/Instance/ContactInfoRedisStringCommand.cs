using AutoMapper;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.ContactInfo;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoRedisStringCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IMapper _mapper;
        private readonly IContactInfoService _contactInfoService;
        private readonly IRedisService _redisService;
        private readonly IRedlockService _redlockService;
        private const string _redisQueryByID = $"{nameof(ContactInfo)}:{nameof(QueryByID)}";
        private const string _redisQueryByCondition = $"{nameof(ContactInfo)}:{nameof(QueryByCondition)}";
        private TimeSpan _redisTTL = TimeSpan.FromMinutes(10);
        private TimeSpan _redisJitter = TimeSpan.FromMinutes(5);
        private TimeSpan _redisEmptyTTL = TimeSpan.FromMinutes(3);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="contactInfoService"></param>
        /// <param name="redisService"></param>
        /// <param name="redlockService"></param>
        public ContactInfoRedisStringCommand(
            IMapper mapper,
            IContactInfoService contactInfoService,
            IRedisService redisService,
            IRedlockService redlockService
        )
        {
            _mapper = mapper;
            _contactInfoService = contactInfoService;
            _redisService = redisService;
            _redlockService = redlockService;
        }

        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> QueryByID(long id)
        {
            var redisKey = $"{_redisQueryByID}:{id}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.GetObject<ContactInfo>(redisKey);

                return res == null ? FailRP<QueryRP>(1, "No Data") : SuccessRP(_mapper.Map<QueryRP>(res));
            }
            else
            {
                var res = _redlockService.AcquireLock(
                            $"Lock:{redisKey}",
                            TimeSpan.FromSeconds(30),
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromMilliseconds(500),
                            () =>
                            {
                                if (_redisService.Exist(redisKey))
                                {
                                    return _redisService.GetObject<ContactInfo>(redisKey);
                                }

                                var dbData = _contactInfoService.Query(id);
                                if (dbData == null)
                                {
                                    _redisService.SetObject(redisKey, (ContactInfo)null, _redisEmptyTTL);
                                }
                                else
                                {
                                    _redisService.SetObjectWithJitter(redisKey, dbData, _redisTTL, _redisJitter);
                                }
                                return dbData;
                            },
                            () =>
                            {
                                throw new TimeoutException("系統忙碌中，請稍後再試");
                            }
                );

                return res == null ? FailRP<QueryRP>(1, "No Data") : SuccessRP(_mapper.Map<QueryRP>(res));
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

            var redisKey = $"{_redisQueryByCondition}:{string.Join('&', dicParams.Select(e => e.Key + "=" + e.Value?.ToString()))}";
            if (_redisService.Exist(redisKey))
            {
                var res = _redisService.GetObject<(int totalCnt, IEnumerable<ContactInfo> data)>(redisKey);

                return SuccessRP(new PageDataRP<IEnumerable<QueryRP>>
                {
                    PageInfo = new PageInfoRP
                    {
                        CurrentIndex = objRQ.PageIndex,
                        CurrentSize = res.data.Count(),
                        PageCnt = (int)Math.Ceiling((decimal)res.totalCnt / objRQ.PageSize),
                        TotalCnt = res.totalCnt
                    },
                    Data = _mapper.Map<IEnumerable<QueryRP>>(res.data)
                });
            }
            else
            {
                var res = _contactInfoService.Query(dicParams);

                if (res.data == null || res.data.Any() == false)
                {
                    return FailRP<PageDataRP<IEnumerable<QueryRP>>>(1, "No Data");
                }
                else
                {
                    _redisService.SetObjectWithJitter(redisKey, res, _redisTTL, _redisJitter);

                    return SuccessRP(new PageDataRP<IEnumerable<QueryRP>>
                    {
                        PageInfo = new PageInfoRP
                        {
                            CurrentIndex = objRQ.PageIndex,
                            CurrentSize = res.data.Count(),
                            PageCnt = (int)Math.Ceiling((decimal)res.totalCnt / objRQ.PageSize),
                            TotalCnt = res.totalCnt
                        },
                        Data = _mapper.Map<IEnumerable<QueryRP>>(res.data)
                    });
                }
            }
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> Create(CreateRQ objRQ)
        {
            var objInsert = _mapper.Map<ContactInfo>(objRQ);

            var res = _contactInfoService.Insert(objInsert);

            if (res == false)
            {
                return FailRP<QueryRP>(2, "Create Fail");
            }
            else
            {
                var objCache = _contactInfoService.Query(objInsert.ContactInfoID);
                _redisService.SetObjectWithJitter($"{_redisQueryByID}:{objCache.ContactInfoID}", objCache, _redisTTL, _redisJitter);
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");

                return SuccessRP(_mapper.Map<QueryRP>(objCache));
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
            var objUpdate = _mapper.Map<ContactInfo>(objRQ);

            var res = _contactInfoService.Update(objUpdate);

            if (res == false)
            {
                return FailRP<QueryRP>(3, "Edit Fail");
            }
            else
            {
                _redisService.RemoveAsync($"{_redisQueryByID}:{objUpdate.ContactInfoID}");
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");

                var resQuery = _contactInfoService.Query(objUpdate.ContactInfoID);
                return SuccessRP(_mapper.Map<QueryRP>(resQuery));
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
            var objUpdate = _mapper.Map<ContactInfo>(objOrigin);
            _mapper.Map(objRQ, objUpdate);

            var res = _contactInfoService.Update(objUpdate);

            if (res == false)
            {
                return FailRP<QueryRP>(3, "Edit Partial Fail");
            }
            else
            {
                _redisService.RemoveAsync($"{_redisQueryByID}:{objUpdate.ContactInfoID}");
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");

                var resQuery = _contactInfoService.Query(objUpdate.ContactInfoID);
                return SuccessRP(_mapper.Map<QueryRP>(resQuery));
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

            if (res == false)
            {
                return FailRP<bool>(4, "Remove Fail");
            }
            else
            {
                _redisService.RemoveAsync(ids.Select(e => $"{_redisQueryByID}:{e}"));
                _redisService.RemoveByKeyAsync($"{_redisQueryByCondition}");

                return SuccessRP(res);
            }
        }
    }
}
