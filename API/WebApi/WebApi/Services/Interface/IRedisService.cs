namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        void Set(string key, object value, TimeSpan ts);

        /// <summary>
        /// 獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string key);

        /// <summary>
        /// 判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exist(string key);

        /// <summary>
        /// 移除某個Key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 清空
        /// </summary>
        void Clear();

        /// <summary>
        /// 異步設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        Task SetAsync(string key, object value, TimeSpan ts);

        /// <summary>
        /// 異步獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// 異步獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync<TEntity>(string key);

        /// <summary>
        /// 異步判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(string key);

        /// <summary>
        /// 異步移除某個Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// 異步移除模糊查詢到的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveByKeyAsync(string key);

        /// <summary>
        /// 異步清空
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();
    }
}
