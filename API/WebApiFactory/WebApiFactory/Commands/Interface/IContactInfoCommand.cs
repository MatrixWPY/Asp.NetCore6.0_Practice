using WebApiFactory.Models.Data;
using WebApiFactory.Models.Request;
using WebApiFactory.Models.Response;

namespace WebApiFactory.Commands.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContactInfoCommand
    {
        /// <summary>
        /// 單筆查詢
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        ApiResultRP<ContactInfo> QueryByID(long id, string serviceType);

        /// <summary>
        /// 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<PageDataRP<IEnumerable<ContactInfo>>> QueryByCondition(ContactInfoQueryRQ objRQ);

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<ContactInfo> Add(ContactInfoAddRQ objRQ);

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<ContactInfo> Edit(ContactInfoEditRQ objRQ);

        /// <summary>
        /// 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<ContactInfo> EditPartial(ContactInfoEditPartialRQ objRQ);

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="liID"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        ApiResultRP<bool> DeleteByID(IEnumerable<long> liID, string serviceType);
    }
}
