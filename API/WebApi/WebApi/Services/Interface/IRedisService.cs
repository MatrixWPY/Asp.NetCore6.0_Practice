﻿namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedisService
    {
        #region Common
        /// <summary>
        /// 清空緩存值
        /// </summary>
        void Clear();

        /// <summary>
        /// 異步清空緩存值
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// 移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        void Remove(string redisKey);

        /// <summary>
        /// 異步移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task RemoveAsync(string redisKey);

        /// <summary>
        /// 移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        void Remove(IEnumerable<string> redisKeys);

        /// <summary>
        /// 異步移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        Task RemoveAsync(IEnumerable<string> redisKeys);

        /// <summary>
        /// 移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        void RemoveByKey(string keyPrefix);

        /// <summary>
        /// 異步移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        Task RemoveByKeyAsync(string keyPrefix);

        /// <summary>
        /// 判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        bool Exist(string redisKey);

        /// <summary>
        /// 異步判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(string redisKey);
        #endregion

        #region String
        /// <summary>
        /// 獲取String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        string GetString(string redisKey);

        /// <summary>
        /// 異步獲取String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string redisKey);

        /// <summary>
        /// 設置String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        void SetString(string redisKey, string redisValue, TimeSpan tsExpiry);

        /// <summary>
        /// 異步設置String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        Task SetStringAsync(string redisKey, string redisValue, TimeSpan tsExpiry);

        /// <summary>
        /// 獲取Deserialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        T GetObject<T>(string redisKey);

        /// <summary>
        /// 異步獲取Deserialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string redisKey);

        /// <summary>
        /// 設置Serialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        void SetObject<T>(string redisKey, T redisValue, TimeSpan tsExpiry);

        /// <summary>
        /// 異步設置Serialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        Task SetObjectAsync<T>(string redisKey, T redisValue, TimeSpan tsExpiry);
        #endregion

        #region Hash
        /// <summary>
        /// 判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        bool ExistHash<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 異步判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task<bool> ExistHashAsync<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        string GetHash<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 異步獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task<string> GetHashAsync<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <param name="hashValue"></param>
        /// <param name="tsExpiry"></param>
        void SetHash<TKey>(string redisKey, TKey hashKey, string hashValue, TimeSpan tsExpiry);

        /// <summary>
        /// 異步設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <param name="hashValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        Task SetHashAsync<TKey>(string redisKey, TKey hashKey, string hashValue, TimeSpan tsExpiry);

        /// <summary>
        /// 獲取Hash Deserialize緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        IDictionary<TKey, TValue> GetHashObject<TKey, TValue>(string redisKey);

        /// <summary>
        /// 異步獲取Hash Deserialize緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<IDictionary<TKey, TValue>> GetHashObjectAsync<TKey, TValue>(string redisKey);

        /// <summary>
        /// 獲取Hash Deserialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        TValue GetHashObject<TKey, TValue>(string redisKey, TKey hashKey);

        /// <summary>
        /// 異步獲取Hash Deserialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task<TValue> GetHashObjectAsync<TKey, TValue>(string redisKey, TKey hashKey);

        /// <summary>
        /// 設置Hash Serialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsExpiry"></param>
        void SetHashObject<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsExpiry);

        /// <summary>
        /// 異步設置Hash Serialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        Task SetHashObjectAsync<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsExpiry);

        /// <summary>
        /// 移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        void DeleteHash<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 異步移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task DeleteHashAsync<TKey>(string redisKey, TKey hashKey);

        /// <summary>
        /// 移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeys"></param>
        void DeleteHash<TKey>(string redisKey, IEnumerable<TKey> hashKeys);

        /// <summary>
        /// 異步移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeys"></param>
        /// <returns></returns>
        Task DeleteHashAsync<TKey>(string redisKey, IEnumerable<TKey> hashKeys);
        #endregion

        #region StreamQueue
        /// <summary>
        /// 獲取資料從Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="consumerName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        IDictionary<TKey, TValue> ReceiveStreamQueue<TKey, TValue>(string queueName, string groupName, string consumerName, int maxCount);

        /// <summary>
        /// 異步獲取資料從Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="consumerName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        Task<IDictionary<TKey, TValue>> ReceiveStreamQueueAsync<TKey, TValue>(string queueName, string groupName, string consumerName, int maxCount);

        /// <summary>
        /// 設置資料至Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        void SendStreamQueue<TKey, TValue>(string queueName, string groupName, TKey redisKey, TValue redisValue);

        /// <summary>
        /// 異步設置資料至Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        Task SendStreamQueueAsync<TKey, TValue>(string queueName, string groupName, TKey redisKey, TValue redisValue);
        #endregion
    }
}
