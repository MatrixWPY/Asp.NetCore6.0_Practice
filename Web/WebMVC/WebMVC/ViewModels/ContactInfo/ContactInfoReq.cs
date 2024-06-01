﻿using System.ComponentModel.DataAnnotations;
using WebMVC.ViewModels.Common;

namespace WebMVC.ViewModels.ContactInfo
{
    public class QueryReq
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        [Range(0, 1, ErrorMessage = "{0} 限定為0或1。")]
        public short? Gender { get; set; }
    }

    public class CreateReq
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        [Range(0, 1, ErrorMessage = "{0} 限定為0或1。")]
        public short? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "{0} 限定為1-3個數字。")]
        public short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [RegularExpression("[0-9]{6,20}", ErrorMessage = "{0} 限定為6-20個數字。")]
        public string? PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [MaxLength(100, ErrorMessage = "{0} 限定最大長度為100。")]
        public string? Address { get; set; }
    }

    public class EditReq : IdReq
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        [MaxLength(10, ErrorMessage = "{0} 限定最大長度為10。")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        [Range(0, 1, ErrorMessage = "{0} 限定為0或1。")]
        public short? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "{0} 限定為1-3個數字。")]
        public short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [RegularExpression("[0-9]{6,20}", ErrorMessage = "{0} 限定為6-20個數字。")]
        public string? PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        [MaxLength(100, ErrorMessage = "{0} 限定最大長度為100。")]
        public string? Address { get; set; }
    }
}
