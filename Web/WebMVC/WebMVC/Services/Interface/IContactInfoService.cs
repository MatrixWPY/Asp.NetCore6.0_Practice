using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Services.Interface
{
    public interface IContactInfoService
    {
        Task<QueryRes> QueryByIdAsync(long id);

        Task<IEnumerable<QueryRes>> QueryByConditionAsync(QueryReq req);

        Task<bool> CreateAsync(CreateReq req);

        Task<bool> EditAsync(EditReq req);

        Task<bool> RemoveAsync(IEnumerable<long> ids);
    }
}
