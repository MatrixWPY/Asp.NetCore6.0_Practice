using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using WebApiFactory.Models.Data;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoMysqlSPService : IContactInfoService
    {
        private readonly ILogger<ContactInfoMysqlSPService> _logger;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mySqlConnection"></param>
        public ContactInfoMysqlSPService(ILogger<ContactInfoMysqlSPService> logger, MySqlConnection mySqlConnection)
        {
            _logger = logger;
            _dbConnection = mySqlConnection;
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
                        "Sp_GetContactInfo",
                        new { In_ContactInfoID = id },
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
                            $"Sp_ListContactInfo",
                            new
                            {
                                In_Name = dicParams.GetValueOrDefault("Name"),
                                In_Nickname = dicParams.GetValueOrDefault("Nickname"),
                                In_Gender = dicParams.GetValueOrDefault("Gender"),
                                In_RowStart = dicParams.GetValueOrDefault("RowStart"),
                                In_RowLength = dicParams.GetValueOrDefault("RowLength"),
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
                                                "Sp_AddContactInfo",
                                                new
                                                {
                                                    In_Name = objContactInfo.Name,
                                                    In_Nickname = objContactInfo.Nickname,
                                                    In_Gender = objContactInfo.Gender,
                                                    In_Age = objContactInfo.Age,
                                                    In_PhoneNo = objContactInfo.PhoneNo,
                                                    In_Address = objContactInfo.Address
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
                        "Sp_EditContactInfo",
                        new
                        {
                            In_ContactInfoID = objContactInfo.ContactInfoID,
                            In_Name = objContactInfo.Name,
                            In_Nickname = objContactInfo.Nickname,
                            In_Gender = objContactInfo.Gender,
                            In_Age = objContactInfo.Age,
                            In_PhoneNo = objContactInfo.PhoneNo,
                            In_Address = objContactInfo.Address
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
                        "Sp_RemoveContactInfo",
                        new { In_ContactInfoIDs = string.Join(",", liID) },
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
