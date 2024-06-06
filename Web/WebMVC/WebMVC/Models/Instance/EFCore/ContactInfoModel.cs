using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebMVC.Models.Interface;

namespace WebMVC.Models.Instance.EFCore
{
    [Table("Tbl_ContactInfo")]
    public class ContactInfoModel : IContactInfoModel
    {
        /// <summary>
        /// PKey
        /// </summary>
        [Key]
        [Column(TypeName = "BIGINT")]
        public long ContactInfoID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(10)]
        [Column(TypeName = "NVARCHAR")]
        public string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        [MaxLength(10)]
        [Column(TypeName = "NVARCHAR")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        [Column(TypeName = "TINYINT")]
        public EnumGender? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        [Column(TypeName = "TINYINT")]
        public short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Column(TypeName = "VARCHAR")]
        public string? PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string? Address { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Required]
        [Column(TypeName = "BIT")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        [Column(TypeName = "DATETIME")]
        public DateTime? UpdateTime { get; set; }
    }
}
