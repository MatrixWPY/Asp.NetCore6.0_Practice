using WebApiFactory.Commands.Interface;
using WebApiFactory.Factories.Interface;
using WebApiFactory.Models.Data;
using WebApiFactory.Models.Request;
using WebApiFactory.Models.Response;

namespace WebApiFactory.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IServiceFactory _serviceFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceFactory"></param>
        public ContactInfoCommand(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ApiResultRP<QueryRP> QueryByID(long id, string serviceType)
        {
            var service = _serviceFactory.CreateContactInfoService(serviceType);
            var res = service.Query(id);

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

            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var res = service.Query(dicParams);

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
            var objInsert = new ContactInfo()
            {
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (ContactInfo.EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address
            };

            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var resInsert = service.Insert(objInsert);

            if (resInsert == false)
            {
                return FailRP<QueryRP>(2, "Create Fail");
            }
            else
            {
                var resQuery = service.Query(objInsert.ContactInfoID);

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
            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var objOrigin = service.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
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

            var resUpdate = service.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Fail");
            }
            else
            {
                var resQuery = service.Query(objUpdate.ContactInfoID);

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
            var service = _serviceFactory.CreateContactInfoService(objRQ.ServiceType);
            var objOrigin = service.Query(objRQ.ID ?? 0);
            if (objOrigin == null)
            {
                return FailRP<QueryRP>(1, "No Data");
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

            var resUpdate = service.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Partial Fail");
            }
            else
            {
                var resQuery = service.Query(objUpdate.ContactInfoID);

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
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ApiResultRP<bool> Remove(IEnumerable<long> ids, string serviceType)
        {
            var service = _serviceFactory.CreateContactInfoService(serviceType);
            var res = service.Delete(ids);
            return res == false ? FailRP<bool>(4, "Remove Fail") : SuccessRP(res);
        }
    }
}
