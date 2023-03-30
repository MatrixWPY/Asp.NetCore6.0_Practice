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
                        "CALL Sp_GetContactInfo(?In_ContactInfoID)",
                        new { In_ContactInfoID = id });
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
                            $"CALL Sp_ListContactInfo(?In_Name, ?In_Nickname, ?In_Gender, ?In_RowStart, ?In_RowLength)",
                            new
                            {
                                In_Name = dicParams.GetValueOrDefault("Name"),
                                In_Nickname = dicParams.GetValueOrDefault("Nickname"),
                                In_Gender = dicParams.GetValueOrDefault("Gender"),
                                In_RowStart = dicParams.GetValueOrDefault("RowStart"),
                                In_RowLength = dicParams.GetValueOrDefault("RowLength"),
                            });
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
                                                "CALL Sp_AddContactInfo(?In_Name, ?In_Nickname, ?In_Gender, ?In_Age, ?In_PhoneNo, ?In_Address)",
                                                new
                                                {
                                                    In_Name = objContactInfo.Name,
                                                    In_Nickname = objContactInfo.Nickname,
                                                    In_Gender = objContactInfo.Gender,
                                                    In_Age = objContactInfo.Age,
                                                    In_PhoneNo = objContactInfo.PhoneNo,
                                                    In_Address = objContactInfo.Address
                                                }) ?? 0;
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
                        "CALL Sp_EditContactInfo(?In_ContactInfoID, ?In_Name, ?In_Nickname, ?In_Gender, ?In_Age, ?In_PhoneNo, ?In_Address)",
                        new
                        {
                            In_ContactInfoID = objContactInfo.ContactInfoID,
                            In_Name = objContactInfo.Name,
                            In_Nickname = objContactInfo.Nickname,
                            In_Gender = objContactInfo.Gender,
                            In_Age = objContactInfo.Age,
                            In_PhoneNo = objContactInfo.PhoneNo,
                            In_Address = objContactInfo.Address
                        }) > 0;
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
                        "CALL Sp_RemoveContactInfo(?In_ContactInfoIDs)",
                        new { In_ContactInfoIDs = string.Join(",", liID) }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                return false;
            }
        }
    }
}
