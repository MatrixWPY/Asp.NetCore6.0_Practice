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
    public class ContactInfoCommand : BaseCommand, IContactInfoCommand
    {
        private readonly IMapper _mapper;
        private readonly IContactInfoService _contactInfoService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="contactInfoService"></param>
        public ContactInfoCommand(
            IMapper mapper,
            IContactInfoService contactInfoService
        )
        {
            _mapper = mapper;
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
                return SuccessRP(_mapper.Map<QueryRP>(res));
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
                    Data = _mapper.Map<IEnumerable<QueryRP>>(res.data)
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
            var objInsert = _mapper.Map<ContactInfo>(objRQ);

            var resInsert = _contactInfoService.Insert(objInsert);

            if (resInsert == false)
            {
                return FailRP<QueryRP>(2, "Create Fail");
            }
            else
            {
                var resQuery = _contactInfoService.Query(objInsert.ContactInfoID);

                return SuccessRP(_mapper.Map<QueryRP>(resQuery));
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

            var resUpdate = _contactInfoService.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Fail");
            }
            else
            {
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

            var resUpdate = _contactInfoService.Update(objUpdate);

            if (resUpdate == false)
            {
                return FailRP<QueryRP>(3, "Edit Partial Fail");
            }
            else
            {
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
            return res == false ? FailRP<bool>(4, "Remove Fail") : SuccessRP(res);
        }
    }
}
