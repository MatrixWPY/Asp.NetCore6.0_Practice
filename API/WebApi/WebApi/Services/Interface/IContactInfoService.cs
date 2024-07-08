using WebApi.Models.Data;

namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContactInfoService
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactInfo Query(long id);

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="dicParams"></param>
        /// <returns></returns>
        (int totalCnt, IEnumerable<ContactInfo> data) Query(Dictionary<string, object> dicParams);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="objContactInfo"></param>
        /// <returns></returns>
        bool Insert(ContactInfo objContactInfo);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="objContactInfo"></param>
        /// <returns></returns>
        bool Update(ContactInfo objContactInfo);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="liID"></param>
        /// <returns></returns>
        bool Delete(IEnumerable<long> liID);
    }
}
