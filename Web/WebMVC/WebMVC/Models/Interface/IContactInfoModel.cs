using WebMVC.Models.Common;

namespace WebMVC.Models.Interface
{
    public interface IContactInfoModel
    {
        /// <summary>
        /// PKey
        /// </summary>
        long ContactInfoID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        string? Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        EnumGender? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        string? PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        string? Address { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        bool IsEnable { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        DateTime? UpdateTime { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
