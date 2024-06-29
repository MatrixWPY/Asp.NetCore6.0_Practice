namespace WebMVC.ViewModels.Common
{
    /// <summary>
    /// 傳出參數 - 分頁資訊
    /// </summary>
    public class PageInfoRes
    {
        /// <summary>
        /// 目前頁碼
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 目前數量
        /// </summary>
        public int CurrentSize { get; set; }

        /// <summary>
        /// 分頁總數
        /// </summary>
        public int PageCnt { get; set; }

        /// <summary>
        /// 資料總數
        /// </summary>
        public int TotalCnt { get; set; }
    }

    /// <summary>
    /// 傳出參數 - 分頁資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDataRes<T>
    {
        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PageInfoRes PageInfo { get; set; }

        /// <summary>
        /// 資料物件
        /// </summary>
        public T Data { get; set; }
    }
}
