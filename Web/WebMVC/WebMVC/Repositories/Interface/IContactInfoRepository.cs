using WebMVC.Models.Interface;

namespace WebMVC.Repositories.Interface
{
    public interface IContactInfoRepository
    {
        Task<IContactInfoModel?> QueryAsync(long id);

        Task<(int totalCnt,IEnumerable<IContactInfoModel> data)> QueryAsync(Dictionary<string, object> dicParams);

        Task<bool> InsertAsync(IContactInfoModel contactInfo);

        Task<(bool result, string errorMsg)> UpdateAsync(IContactInfoModel contactInfo);

        Task<bool> DeleteAsync(IEnumerable<long> ids);
    }
}
