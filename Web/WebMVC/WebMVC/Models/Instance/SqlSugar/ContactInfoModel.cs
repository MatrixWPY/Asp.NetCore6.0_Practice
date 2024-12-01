﻿using SqlSugar;
using WebMVC.Models.Common;
using WebMVC.Models.Interface;

namespace WebMVC.Models.Instance.SqlSugar
{
    [SugarTable("Tbl_ContactInfo")]
    public class ContactInfoModel : IContactInfoModel
    {
        /// <summary>
        /// PKey
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long ContactInfoID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        public EnumGender? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        public short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        public string? PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public byte[] RowVersion { get; set; }
    }
}
