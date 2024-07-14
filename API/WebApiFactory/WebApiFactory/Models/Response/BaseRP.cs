namespace WebApiFactory.Models.Response
{
    /// <summary>
    /// 傳出參數 - API回傳格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResultRP<T>
    {
        /// <summary>
        /// 回傳代碼
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 回傳資料
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// 傳出參數 - 分頁資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDataRP<T>
    {
        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PageInfoRP PageInfo { get; set; }

        /// <summary>
        /// 資料物件
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 傳出參數 - 分頁資訊
    /// </summary>
    public class PageInfoRP
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
}
