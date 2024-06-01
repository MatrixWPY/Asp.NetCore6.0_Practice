namespace WebMVC.ViewModels.Common
{
    /// <summary>
    /// 傳出參數 - 分頁資訊
    /// </summary>
    public class PageInfoRes
    {
        /// <summary>
        /// 分頁頁碼
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 資料數量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 分頁總數
        /// </summary>
        public int PageCnt { get; set; }

        /// <summary>
        /// 資料總數
        /// </summary>
        public int TotalCnt { get; set; }
    }
}
