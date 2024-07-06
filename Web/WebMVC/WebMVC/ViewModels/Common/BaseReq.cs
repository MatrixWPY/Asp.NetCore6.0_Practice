using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels.Common
{
    /// <summary>
    /// 傳入參數 - PKey
    /// </summary>
    public class IdReq
    {
        /// <summary>
        /// PKey
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        public long? Id { get; set; }
    }

    /// <summary>
    /// 傳入參數 - PKeys
    /// </summary>
    public class IdsReq
    {
        /// <summary>
        /// PKeys
        /// </summary>
        [Required(ErrorMessage = "{0} 為必填欄位。")]
        public IEnumerable<long> Ids { get; set; }
    }

    /// <summary>
    /// 傳入參數 - 分頁資訊
    /// </summary>
    public class PageInfoReq
    {
        private int _PageIndex = 1;
        private int _PageSize = 10;

        /// <summary>
        /// 分頁頁碼
        /// </summary>
        public int PageIndex
        {
            get => _PageIndex;
            set => _PageIndex = (value < 1 ? 1 : value);
        }

        /// <summary>
        /// 資料數量
        /// </summary>
        public int PageSize
        {
            get => _PageSize;
            set => _PageSize = (value < 1 ? 10 : value);
        }
    }
}
