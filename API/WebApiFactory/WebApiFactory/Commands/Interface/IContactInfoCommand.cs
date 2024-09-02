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
        ApiResultRP<QueryRP> QueryByID(long id, string serviceType);

        /// <summary>
        /// 多筆查詢
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<PageDataRP<IEnumerable<QueryRP>>> QueryByCondition(QueryRQ objRQ);

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<QueryRP> Create(CreateRQ objRQ);

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<QueryRP> Edit(EditRQ objRQ);

        /// <summary>
        /// 部分修改資料
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        ApiResultRP<QueryRP> EditPartial(EditPartialRQ objRQ);

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        ApiResultRP<bool> Remove(IEnumerable<long> ids, string serviceType);
    }
}
