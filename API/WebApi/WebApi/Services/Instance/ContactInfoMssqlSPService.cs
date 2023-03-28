using Dapper;
using System.Data;
using WebApi.Models.Data;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoMssqlSPService : IContactInfoService
    {
        private readonly ILogger<ContactInfoMssqlSPService> _logger;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbConnection"></param>
        public ContactInfoMssqlSPService(ILogger<ContactInfoMssqlSPService> logger, IDbConnection dbConnection)
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
                return _dbConnection.QueryFirstOrDefault<ContactInfo>(
                        "EXEC dbo.Sp_GetContactInfo @ContactInfoID = @ContactInfoID",
                        new { ContactInfoID = id });
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
                var res = _dbConnection.QueryMultiple(
                            $"EXEC dbo.Sp_ListContactInfo {string.Join(",", dicParams.Select(e => $"@{e.Key} = @{e.Key}"))}",
                            dicParams);
                return (res.Read<int>().FirstOrDefault(), res.Read<ContactInfo>());
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
                objContactInfo.ContactInfoID = _dbConnection.ExecuteScalar<long?>(
                                                "EXEC dbo.Sp_AddContactInfo @Name = @Name, @Nickname = @Nickname, @Gender = @Gender, @Age = @Age, @PhoneNo = @PhoneNo, @Address = @Address",
                                                objContactInfo) ?? 0;
                return objContactInfo.ContactInfoID > 0;
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
                return _dbConnection.Execute(
                        "EXEC dbo.Sp_EditContactInfo @ContactInfoID = @ContactInfoID, @Name = @Name, @Nickname = @Nickname, @Gender = @Gender, @Age = @Age, @PhoneNo = @PhoneNo, @Address = @Address",
                        objContactInfo) > 0;
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
                return _dbConnection.Execute(
                        "EXEC dbo.Sp_RemoveContactInfo @ContactInfoIDs = @ContactInfoIDs",
                        new { ContactInfoIDs = string.Join(",", liID) }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                return false;
            }
        }
    }
}
