namespace WebApi.Models.Response
{
    /// <summary>
    /// 傳出參數 - 查詢
    /// </summary>
    public class QueryRP
    {
        /// <summary>
        /// PKey
        /// </summary>
        public long ContactInfoID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 性別 (0:Female, 1:Male)
        /// </summary>
        public short? Gender { get; set; }

        /// <summary>
        /// 年齡
        /// </summary>
        public short? Age { get; set; }

        /// <summary>
        /// 電話號碼
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }
    }
}
