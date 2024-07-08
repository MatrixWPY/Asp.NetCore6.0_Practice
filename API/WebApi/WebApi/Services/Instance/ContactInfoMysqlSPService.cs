using Dapper;
using System.Data;
using WebApi.Models.Data;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
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
        /// <param name="dbConnection"></param>
        public ContactInfoMysqlSPService(ILogger<ContactInfoMysqlSPService> logger, IDbConnection dbConnection)
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
                        "Sp_GetContactInfo",
                        new { In_ContactInfoID = id },
                        commandType: CommandType.StoredProcedure);
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
                        "Sp_RemoveContactInfo",
                        new { In_ContactInfoIDs = string.Join(",", liID) },
                        commandType: CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                return false;
            }
        }
    }
}
