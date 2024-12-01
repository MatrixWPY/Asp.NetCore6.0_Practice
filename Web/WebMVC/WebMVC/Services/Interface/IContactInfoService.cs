using WebMVC.ViewModels.Common;
using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Services.Interface
{
    public interface IContactInfoService
    {
        Task<QueryRes> QueryByIdAsync(long id);

        Task<PageDataRes<IEnumerable<QueryRes>>> QueryByConditionAsync(QueryReq req);

        Task<bool> CreateAsync(CreateReq req);

        Task<(bool result, string errorMsg)> EditAsync(EditReq req);

        Task<bool> RemoveAsync(IEnumerable<long> ids);
    }
}
