using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels.ContactInfo
{
    public class QueryRes
    {
        [Display(Name = "流水號")]
        public long ContactInfoID { get; set; }

        [Display(Name = "姓名")]
        public string? Name { get; set; }

        [Display(Name = "暱稱")]
        public string? Nickname { get; set; }

        [Display(Name = "性別")]
        public short? Gender { get; set; }

        [Display(Name = "年齡")]
        public short? Age { get; set; }

        [Display(Name = "電話號碼")]
        public string? PhoneNo { get; set; }

        [Display(Name = "住址")]
        public string? Address { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
