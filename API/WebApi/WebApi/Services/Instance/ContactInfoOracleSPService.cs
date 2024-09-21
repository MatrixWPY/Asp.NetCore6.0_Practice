using Dapper;
using Dapper.Oracle;
using System.Data;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoOracleSPService : IContactInfoService
    {
        private readonly ILogger<ContactInfoOracleSPService> _logger;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbConnection"></param>
        public ContactInfoOracleSPService(ILogger<ContactInfoOracleSPService> logger, IDbConnection dbConnection)
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
                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("i_contactInfoID", id);
                dynamicParam.Add("o_result", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                return _dbConnection.QueryFirstOrDefault<ContactInfo>(
                    "SP_GET_CONTACTINFO",
                    dynamicParam,
                    commandType: CommandType.StoredProcedure
                );
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
                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("i_name", dicParams.GetValueOrDefault("Name"));
                dynamicParam.Add("i_nickname", dicParams.GetValueOrDefault("Nickname"));
                dynamicParam.Add("i_gender", dicParams.GetValueOrDefault("Gender"));
                dynamicParam.Add("i_rowStart", dicParams.GetValueOrDefault("RowStart"));
                dynamicParam.Add("i_rowLength", dicParams.GetValueOrDefault("RowLength"));
                dynamicParam.Add("o_cnt", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                dynamicParam.Add("o_result", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var res = _dbConnection.QueryMultiple(
                    "SP_QUERY_CONTACTINFO",
                    dynamicParam,
                    commandType: CommandType.StoredProcedure
                );

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
                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("i_name", objContactInfo.Name);
                dynamicParam.Add("i_nickname", objContactInfo.Nickname);
                dynamicParam.Add("i_gender", (int?)objContactInfo.Gender);
                dynamicParam.Add("i_age", objContactInfo.Age);
                dynamicParam.Add("i_phoneNo", objContactInfo.PhoneNo);
                dynamicParam.Add("i_address", objContactInfo.Address);
                dynamicParam.Add("o_contactInfoID", dbType: OracleMappingType.Int64, direction: ParameterDirection.Output);

                _dbConnection.Execute(
                    "SP_INSERT_CONTACTINFO",
                    dynamicParam,
                    commandType: CommandType.StoredProcedure
                );
                objContactInfo.ContactInfoID = dynamicParam.Get<long?>("o_contactInfoID") ?? 0;

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
                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("i_contactInfoID", objContactInfo.ContactInfoID);
                dynamicParam.Add("i_name", objContactInfo.Name);
                dynamicParam.Add("i_nickname", objContactInfo.Nickname);
                dynamicParam.Add("i_gender", (int?)objContactInfo.Gender);
                dynamicParam.Add("i_age", objContactInfo.Age);
                dynamicParam.Add("i_phoneNo", objContactInfo.PhoneNo);
                dynamicParam.Add("i_address", objContactInfo.Address);

                _dbConnection.Execute(
                    "SP_UPDATE_CONTACTINFO",
                    dynamicParam,
                    commandType: CommandType.StoredProcedure
                );

                return true;
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
                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("i_contactInfoIDs", string.Join(",", ids));

                _dbConnection.Execute(
                    "SP_DELETE_CONTACTINFO",
                    dynamicParam,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                throw;
            }
        }
    }
}
