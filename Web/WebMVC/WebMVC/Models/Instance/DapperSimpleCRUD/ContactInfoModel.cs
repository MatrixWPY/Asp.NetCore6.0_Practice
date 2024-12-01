using Dapper;
using WebMVC.Models.Common;
using WebMVC.Models.Interface;

namespace WebMVC.Models.Instance.DapperSimpleCRUD
{
    [Table("Tbl_ContactInfo")]
    public class ContactInfoModel : IContactInfoModel
    {
        /// <summary>
        /// PKey
        /// </summary>
        [Key]
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
        [IgnoreUpdate]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        [IgnoreInsert]
        public DateTime? UpdateTime { get; set; }

        [IgnoreInsert]
        [IgnoreUpdate]
        public byte[] RowVersion { get; set; }
    }
}
