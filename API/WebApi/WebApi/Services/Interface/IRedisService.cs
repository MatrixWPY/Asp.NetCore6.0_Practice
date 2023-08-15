namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedisService
    {
        #region Common
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();

        /// <summary>
        /// 異步清空
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// 移除某個Key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

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
        /// 判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exist(string key);

        /// <summary>
        /// 異步判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(string key);
        #endregion

        #region String
        /// <summary>
        /// 獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 異步獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// 獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string key);

        /// <summary>
        /// 異步獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync<TEntity>(string key);

        /// <summary>
        /// 設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        void Set(string key, object value, TimeSpan ts);

        /// <summary>
        /// 異步設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        Task SetAsync(string key, object value, TimeSpan ts);
        #endregion

        #region Hash
        /// <summary>
        /// 判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        bool HashExist<TKey>(string redisKey, TKey dictKey);

        /// <summary>
        /// 異步判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        Task<bool> HashExistAsync<TKey>(string redisKey, TKey dictKey);

        /// <summary>
        /// 獲取Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Dictionary<TKey, TValue> HashGet<TKey, TValue>(string redisKey);

        /// <summary>
        /// 異步獲取Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<Dictionary<TKey, TValue>> HashGetAsync<TKey, TValue>(string redisKey);

        /// <summary>
        /// 獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        TValue HashGet<TKey, TValue>(string redisKey, TKey dictKey);

        /// <summary>
        /// 異步獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        Task<TValue> HashGetAsync<TKey, TValue>(string redisKey, TKey dictKey);

        /// <summary>
        /// 設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKeyValue"></param>
        /// <param name="ts"></param>
        void HashSet<TKey, TValue>(string redisKey, Dictionary<TKey, TValue> dictKeyValue, TimeSpan ts);

        /// <summary>
        /// 異步設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKeyValue"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        Task HashSetAsync<TKey, TValue>(string redisKey, Dictionary<TKey, TValue> dictKeyValue, TimeSpan ts);

        /// <summary>
        /// 移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        void HashDelete<TKey>(string redisKey, TKey dictKey);

        /// <summary>
        /// 異步移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        Task HashDeleteAsync<TKey>(string redisKey, TKey dictKey);

        /// <summary>
        /// 移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKeys"></param>
        void HashDelete<TKey>(string redisKey, IEnumerable<TKey> dictKeys);

        /// <summary>
        /// 異步移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="dictKeys"></param>
        /// <returns></returns>
        Task HashDeleteAsync<TKey>(string redisKey, IEnumerable<TKey> dictKeys);
        #endregion
    }
}
