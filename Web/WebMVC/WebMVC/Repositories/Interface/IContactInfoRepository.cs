using WebMVC.Models.Interface;

namespace WebMVC.Repositories.Interface
{
    public interface IContactInfoRepository
    {
        Task<IContactInfoModel?> QueryAsync(long id);

        Task<IEnumerable<IContactInfoModel>> QueryAsync(Dictionary<string, object> dicParams);

        Task<bool> InsertAsync(IContactInfoModel contactInfo);

        Task<bool> UpdateAsync(IContactInfoModel contactInfo);

        Task<bool> DeleteAsync(IEnumerable<long> ids);
    }
}
