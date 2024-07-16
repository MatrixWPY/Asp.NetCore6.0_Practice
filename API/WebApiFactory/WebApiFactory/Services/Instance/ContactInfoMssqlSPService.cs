using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiFactory.Models.Data;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Services.Instance
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
        /// <param name="sqlConnection"></param>
        public ContactInfoMssqlSPService(ILogger<ContactInfoMssqlSPService> logger, SqlConnection sqlConnection)
        {
            _logger = logger;
            _dbConnection = sqlConnection;
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactInfo? Query(long id)
        {
            try
            {
                return _dbConnection.QueryFirstOrDefault<ContactInfo>(
                        "dbo.Sp_GetContactInfo",
                        new { ContactInfoID = id },
                        commandType: CommandType.StoredProcedure);
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
                var res = _dbConnection.QueryMultiple(
                            $"dbo.Sp_ListContactInfo",
                            new
                            {
                                Name = dicParams.GetValueOrDefault("Name"),
                                Nickname = dicParams.GetValueOrDefault("Nickname"),
                                Gender = dicParams.GetValueOrDefault("Gender"),
                                RowStart = dicParams.GetValueOrDefault("RowStart"),
                                RowLength = dicParams.GetValueOrDefault("RowLength"),
                            },
                            commandType: CommandType.StoredProcedure);
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
                objContactInfo.ContactInfoID = _dbConnection.ExecuteScalar<long?>(
                                                "dbo.Sp_AddContactInfo",
                                                new
                                                {
                                                    objContactInfo.Name,
                                                    objContactInfo.Nickname,
                                                    objContactInfo.Gender,
                                                    objContactInfo.Age,
                                                    objContactInfo.PhoneNo,
                                                    objContactInfo.Address
                                                },
                                                commandType: CommandType.StoredProcedure) ?? 0;
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
                return _dbConnection.Execute(
                        "dbo.Sp_EditContactInfo",
                        new
                        {
                            objContactInfo.ContactInfoID,
                            objContactInfo.Name,
                            objContactInfo.Nickname,
                            objContactInfo.Gender,
                            objContactInfo.Age,
                            objContactInfo.PhoneNo,
                            objContactInfo.Address
                        },
                        commandType: CommandType.StoredProcedure) > 0;
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
        /// <param name="liID"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<long> liID)
        {
            try
            {
                return _dbConnection.Execute(
                        "dbo.Sp_RemoveContactInfo",
                        new { ContactInfoIDs = string.Join(",", liID) },
                        commandType: CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                throw;
            }
        }
    }
}
