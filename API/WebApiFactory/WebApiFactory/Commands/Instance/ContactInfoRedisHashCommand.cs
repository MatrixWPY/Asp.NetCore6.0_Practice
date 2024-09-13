using AutoMapper;
using WebApiFactory.Commands.Interface;
using WebApiFactory.DtoModels.Common;
using WebApiFactory.DtoModels.ContactInfo;
using WebApiFactory.Factories.Interface;
using WebApiFactory.Models;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoRedisHashCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IMapper _mapper;
        private readonly IServiceFactory _serviceFactory;
        private readonly IRedisService _redisService;
        private const string _redisQueryByID = $"{nameof(ContactInfo)}:{nameof(QueryByID)}";
        private const string _redisQueryByCondition = $"{nameof(ContactInfo)}:{nameof(QueryByCondition)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="serviceFactory"></param>
        /// <param name="redisService"></param>
        public ContactInfoRedisHashCommand(
            IMapper mapper,
            IServiceFactory serviceFactory,
            IRedisService redisService
        )
        {
            _mapper = mapper;
            _serviceFactory = serviceFactory;
            _redisService = redisService;
        }

        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> QueryByID(long id, string serviceType)
        {
            if (_redisService.ExistHash(_redisQueryByID, id))
            {
                var res = _redisService.GetHashObject<long, ContactInfo>(_redisQueryByID, id);

                return SuccessRP(_mapper.Map<QueryRP>(res));
            }
            else
            {
                var service = _serviceFactory.CreateContactInfoService(serviceType);
                var res = service.Query(id);

                if (res == null)
                {
                    return FailRP<QueryRP>(1, "No Data");
                }
                else
                {
                    _redisService.SetHashObjectAsync(_redisQueryByID, new Dictionary<long, ContactInfo> { { id, res } }, TimeSpan.FromMinutes(5));

                    return SuccessRP(_mapper.Map<QueryRP>(res));
                }
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

            var hashKey = $"{string.Join('&', dicParams.Select(e => e.Key + "=" + e.Value?.ToString()))}";
            if (_redisService.ExistHash(_redisQueryByCondition, hashKey))
            {
                var res = _redisService.GetHashObject<string, (int totalCnt, IEnumerable<ContactInfo> data)>(_redisQueryByCondition, hashKey);

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
                var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
                var res = service.Query(dicParams);

                if (res.data == null)
                {
                    return FailRP<PageDataRP<IEnumerable<QueryRP>>>(1, "No Data");
                }
                else
                {
                    _redisService.SetHashObjectAsync(_redisQueryByCondition, new Dictionary<string, (int, IEnumerable<ContactInfo>)> { { hashKey, res } }, TimeSpan.FromMinutes(5));

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

            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var res = service.Insert(objInsert);

            if (res == false)
            {
                return FailRP<QueryRP>(2, "Create Fail");
            }
            else
            {
                var objCache = service.Query(objInsert.ContactInfoID);
                _redisService.SetHashObjectAsync(_redisQueryByID, new Dictionary<long, ContactInfo> { { objCache.ContactInfoID, objCache } }, TimeSpan.FromMinutes(5));
                _redisService.RemoveAsync(_redisQueryByCondition);

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
            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var objOrigin = service.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
            }
            var objUpdate = _mapper.Map<ContactInfo>(objRQ);

            var res = service.Update(objUpdate);

            if (res == false)
            {
                return FailRP<QueryRP>(3, "Edit Fail");
            }
            else
            {
                var objCache = service.Query(objUpdate.ContactInfoID);
                _redisService.SetHashObjectAsync(_redisQueryByID, new Dictionary<long, ContactInfo> { { objCache.ContactInfoID, objCache } }, TimeSpan.FromMinutes(5));
                _redisService.RemoveAsync(_redisQueryByCondition);

                return SuccessRP(_mapper.Map<QueryRP>(objCache));
            }
        }

        /// <summary>
        /// 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> EditPartial(EditPartialRQ objRQ)
        {
            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var objOrigin = service.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
            }
            var objUpdate = _mapper.Map<ContactInfo>(objOrigin);
            _mapper.Map(objRQ, objUpdate);

            var res = service.Update(objUpdate);

            if (res == false)
            {
                return FailRP<QueryRP>(3, "Edit Partial Fail");
            }
            else
            {
                var objCache = service.Query(objUpdate.ContactInfoID);
                _redisService.SetHashObjectAsync(_redisQueryByID, new Dictionary<long, ContactInfo> { { objCache.ContactInfoID, objCache } }, TimeSpan.FromMinutes(5));
                _redisService.RemoveAsync(_redisQueryByCondition);

                return SuccessRP(_mapper.Map<QueryRP>(objCache));
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ApiResultRP<bool> Remove(IEnumerable<long> ids, string serviceType)
        {
            var service = _serviceFactory.CreateContactInfoService(serviceType);
            var res = service.Delete(ids);

            if (res == false)
            {
                return FailRP<bool>(4, "Remove Fail");
            }
            else
            {
                _redisService.DeleteHashAsync(_redisQueryByID, ids);
                _redisService.RemoveAsync(_redisQueryByCondition);

                return SuccessRP(res);
            }
        }
    }
}
