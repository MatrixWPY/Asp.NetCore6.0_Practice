using Dapper;
using MySql.Data.MySqlClient;
using System.Text;
using WebApi.Models.Data;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoMysqlService : IContactInfoService
    {
        private readonly ILogger<ContactInfoMysqlService> _logger;
        private readonly string _connectString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public ContactInfoMysqlService(ILogger<ContactInfoMysqlService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectString = configuration.GetValue<string>("ConnectionStrings:MySql");
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactInfo Query(long id)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine("SELECT * FROM Tbl_ContactInfo");
                sbSQL.AppendLine("WHERE ContactInfoID=@ContactInfoID");
                sbSQL.AppendLine("ORDER BY ContactInfoID DESC;");

                using (var db = new MySqlConnection(_connectString))
                {
                    return db.QueryFirstOrDefault<ContactInfo>(sbSQL.ToString(), new { ContactInfoID = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Query Fail : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="dicParams"></param>
        /// <returns></returns>
        public (int, IEnumerable<ContactInfo>) Query(Dictionary<string, object> dicParams)
        {
            try
            {
                var sbCnt = new StringBuilder();
                sbCnt.AppendLine("SELECT COUNT(1) FROM Tbl_ContactInfo WHERE 1 = 1");
                var sbQuery = new StringBuilder();
                sbQuery.AppendLine("SELECT * FROM Tbl_ContactInfo WHERE 1 = 1");

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            sbCnt.AppendLine("AND Name = @Name");
                            sbQuery.AppendLine("AND Name = @Name");
                            break;

                        case "Nickname":
                            sbCnt.AppendLine("AND Nickname LIKE @Nickname");
                            sbQuery.AppendLine("AND Nickname LIKE @Nickname");
                            break;

                        case "Gender":
                            sbCnt.AppendLine("AND Gender = @Gender");
                            sbQuery.AppendLine("AND Gender = @Gender");
                            break;
                    }
                }
                #endregion

                #region [Order]
                sbQuery.AppendLine("ORDER BY ContactInfoID DESC");
                #endregion

                #region[Paging]
                sbQuery.AppendLine("LIMIT @RowStart, @RowLength");
                #endregion

                using (var db = new MySqlConnection(_connectString))
                {
                    var res = db.QueryMultiple($"{sbCnt.ToString()}; {sbQuery.ToString()};", dicParams);
                    return (res.Read<int>().FirstOrDefault(), res.Read<ContactInfo>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Query Fail : {ex.Message}");
                return (0, null);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="objContactInfo"></param>
        /// <returns></returns>
        public bool Insert(ContactInfo objContactInfo)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine("INSERT INTO Tbl_ContactInfo (Name, Nickname, Gender, Age, PhoneNo, Address)");
                sbSQL.AppendLine("VALUES (@Name, @Nickname, @Gender, @Age, @PhoneNo, @Address);");
                sbSQL.AppendLine("SELECT LAST_INSERT_ID();");

                using (var db = new MySqlConnection(_connectString))
                {
                    objContactInfo.ContactInfoID = db.ExecuteScalar<long?>(sbSQL.ToString(), objContactInfo) ?? 0;
                    return objContactInfo.ContactInfoID > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Insert Fail : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="objContactInfo"></param>
        /// <returns></returns>
        public bool Update(ContactInfo objContactInfo)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine("UPDATE Tbl_ContactInfo SET");
                sbSQL.AppendLine("Name = @Name, Nickname = @Nickname, Gender = @Gender, Age = @Age, PhoneNo = @PhoneNo, Address = @Address, UpdateTime = NOW()");
                sbSQL.AppendLine("WHERE ContactInfoID = @ContactInfoID;");

                using (var db = new MySqlConnection(_connectString))
                {
                    return db.Execute(sbSQL.ToString(), objContactInfo) > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update Fail : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="liID"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<long> liID)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine("DELETE FROM Tbl_ContactInfo");
                sbSQL.AppendLine("WHERE ContactInfoID IN @ContactInfoIDs;");

                using (var db = new MySqlConnection(_connectString))
                {
                    return db.Execute(sbSQL.ToString(), new { ContactInfoIDs = liID }) > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                return false;
            }
        }
    }
}
