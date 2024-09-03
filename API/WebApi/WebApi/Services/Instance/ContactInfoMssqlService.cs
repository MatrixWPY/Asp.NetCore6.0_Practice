using Dapper;
using System.Data;
using System.Text;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoMssqlService : IContactInfoService
    {
        private readonly ILogger<ContactInfoMssqlService> _logger;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbConnection"></param>
        public ContactInfoMssqlService(ILogger<ContactInfoMssqlService> logger, IDbConnection dbConnection)
        {
            _logger = logger;
            _dbConnection = dbConnection;
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
                sbSQL.AppendLine("ORDER BY ContactInfoID DESC");

                return _dbConnection.QueryFirstOrDefault<ContactInfo>(sbSQL.ToString(), new { ContactInfoID = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Query Fail : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="dicParams"></param>
        /// <returns></returns>
        public (int totalCnt, IEnumerable<ContactInfo> data) Query(Dictionary<string, object> dicParams)
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
                sbQuery.AppendLine("OFFSET @RowStart ROWS FETCH NEXT @RowLength ROWS ONLY");
                #endregion

                var res = _dbConnection.QueryMultiple(sbCnt.ToString() + sbQuery.ToString(), dicParams);
                return (res.Read<int>().FirstOrDefault(), res.Read<ContactInfo>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Query Fail : {ex.Message}");
                throw;
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
                sbSQL.AppendLine("INSERT INTO dbo.Tbl_ContactInfo (Name, Nickname, Gender, Age, PhoneNo, Address)");
                sbSQL.AppendLine("VALUES (@Name, @Nickname, @Gender, @Age, @PhoneNo, @Address)");
                sbSQL.AppendLine("SELECT SCOPE_IDENTITY()");

                objContactInfo.ContactInfoID = _dbConnection.ExecuteScalar<long?>(sbSQL.ToString(), objContactInfo) ?? 0;
                return objContactInfo.ContactInfoID > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Insert Fail : {ex.Message}");
                throw;
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
                sbSQL.AppendLine("UPDATE dbo.Tbl_ContactInfo SET");
                sbSQL.AppendLine("Name = @Name, Nickname = @Nickname, Gender = @Gender, Age = @Age, PhoneNo = @PhoneNo, Address = @Address, UpdateTime = GETDATE()");
                sbSQL.AppendLine("WHERE ContactInfoID = @ContactInfoID");

                return _dbConnection.Execute(sbSQL.ToString(), objContactInfo) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update Fail : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<long> ids)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine("DELETE FROM dbo.Tbl_ContactInfo");
                sbSQL.AppendLine("WHERE ContactInfoID IN @ContactInfoIDs");

                return _dbConnection.Execute(sbSQL.ToString(), new { ContactInfoIDs = ids }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                throw;
            }
        }
    }
}
