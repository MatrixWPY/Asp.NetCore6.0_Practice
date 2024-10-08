﻿using System.ComponentModel.DataAnnotations;

namespace WebApiFactory.DtoModels.Common
{
    /// <summary>
    /// 傳入參數 - PKey
    /// </summary>
    public class IdRQ
    {
        /// <summary>
        /// PKey
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        public long? ID { get; set; }
    }

    /// <summary>
    /// 傳入參數 - 分頁資訊
    /// </summary>
    public class PageInfoRQ
    {
        private int _PageIndex = 1;
        private int _PageSize = 10;

        /// <summary>
        /// 分頁頁碼
        /// </summary>
        public int PageIndex
        {
            get => _PageIndex;
            set => _PageIndex = value < 1 ? 1 : value;
        }

        /// <summary>
        /// 資料數量
        /// </summary>
        public int PageSize
        {
            get => _PageSize;
            set => _PageSize = value < 1 ? 10 : value;
        }
    }

    public interface IFactoryRQ
    {
        /// <summary>
        /// Command實作 (Normal, RedisString, RedisHash)
        /// </summary>
        string CommandType { get; set; }

        /// <summary>
        /// Service實作 (MsSql, MsSqlSP, MySql, MySqlSP , Oracle , OracleSP)
        /// </summary>
        string ServiceType { get; set; }
    }
}
