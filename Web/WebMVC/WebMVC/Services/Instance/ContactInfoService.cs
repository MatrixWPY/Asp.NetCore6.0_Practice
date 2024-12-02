using AutoMapper;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;
using WebMVC.Services.Interface;
using WebMVC.ViewModels.Common;
using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Services.Instance
{
    public class ContactInfoService : IContactInfoService
    {
        private readonly IMapper _mapper;
        private IContactInfoModel _contactInfoModel;
        private IContactInfoRepository _contactInfoRepository;
        private IRedlockService _redlockService;

        public ContactInfoService(
            IMapper mapper,
            IContactInfoModel contactInfoModel,
            IContactInfoRepository contactInfoRepository,
            IRedlockService redlockService
        )
        {
            _mapper = mapper;
            _contactInfoModel = contactInfoModel;
            _contactInfoRepository = contactInfoRepository;
            _redlockService = redlockService;
        }

        public async Task<QueryRes> QueryByIdAsync(long id)
        {
            var res = await _contactInfoRepository.QueryAsync(id);

            return _mapper.Map<QueryRes>(res);
        }

        public async Task<PageDataRes<IEnumerable<QueryRes>>> QueryByConditionAsync(QueryReq req)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(req.Name) == false)
            {
                dicParams["Name"] = req.Name;
            }
            if (string.IsNullOrWhiteSpace(req.Nickname) == false)
            {
                dicParams["Nickname"] = req.Nickname;
            }
            if (req.Gender.HasValue)
            {
                dicParams["Gender"] = req.Gender;
            }
            dicParams["RowStart"] = (req.PageIndex - 1) * req.PageSize;
            dicParams["RowLength"] = req.PageSize;

            var res = await _contactInfoRepository.QueryAsync(dicParams);

            return new PageDataRes<IEnumerable<QueryRes>>()
            {
                PageInfo = new PageInfoRes()
                {
                    CurrentIndex = req.PageIndex,
                    CurrentSize = res.data.Count(),
                    PageCnt = (res.totalCnt % req.PageSize == 0 ? res.totalCnt / req.PageSize : res.totalCnt / req.PageSize + 1),
                    TotalCnt = res.totalCnt
                },
                Data = _mapper.Map<IEnumerable<QueryRes>>(res.data)
            };
        }

        public async Task<bool> CreateAsync(CreateReq req)
        {
            _mapper.Map(req, _contactInfoModel);
            _contactInfoModel.IsEnable = true;
            _contactInfoModel.CreateTime = DateTime.Now;

            return await _contactInfoRepository.InsertAsync(_contactInfoModel);
        }

        public async Task<(bool result, string errorMsg)> EditAsync(EditReq req)
        {
            var origin = await _contactInfoRepository.QueryAsync(req.Id ?? 0);
            if (origin == null)
            {
                return (false, "資料不存在");
            }

            _mapper.Map(req, origin);
            origin.UpdateTime = DateTime.Now;

            return await _redlockService.AcquireLockAsync(
                $"EditLock_{origin.ContactInfoID}", //lock key
                TimeSpan.FromSeconds(30),           //lock key expiry
                TimeSpan.FromSeconds(15),           //放棄重試時間
                TimeSpan.FromMilliseconds(100),     //重試間隔時間
                async () =>                         //取得lock執行的工作
                {
                    return await _contactInfoRepository.UpdateAsync(origin);
                },
                async () =>                         //未取得lock執行的工作
                {
                    return await Task.FromResult((false, "無法併發修改"));
                }
            );
        }

        public async Task<bool> RemoveAsync(IEnumerable<long> ids)
        {
            return await _contactInfoRepository.DeleteAsync(ids);
        }
    }
}
