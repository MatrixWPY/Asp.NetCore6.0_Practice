using Dapper;
using Dapper.Oracle;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;
using WebApiFactory.Models;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoOracleService : IContactInfoService
    {
        private readonly ILogger<ContactInfoOracleService> _logger;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbConnection"></param>
        public ContactInfoOracleService(ILogger<ContactInfoOracleService> logger, OracleConnection oracleConnection)
        {
            _logger = logger;
            _dbConnection = oracleConnection;
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
                var dynamicParam = new OracleDynamicParameters();

                sbSQL.AppendLine(@"
                    SELECT
                        CONTACTINFO_ID AS ContactInfoID,
                        NAME,
                        NICKNAME,
                        GENDER,
                        AGE,
                        PHONE_NO AS PhoneNo,
                        ADDRESS,
                        IS_ENABLE AS IsEnable,
                        CREATE_TIME AS CreateTime,
                        UPDATE_TIME AS UpdateTime
                    FROM
                        TBL_CONTACTINFO
                    WHERE
                        CONTACTINFO_ID = :CONTACTINFO_ID
                ");
                dynamicParam.Add("CONTACTINFO_ID", id);

                return _dbConnection.QueryFirstOrDefault<ContactInfo>(sbSQL.ToString(), dynamicParam);
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

                var sbCnt = new StringBuilder();
                sbCnt.AppendLine(@"
                    OPEN :o_Cnt FOR
                    SELECT
                        COUNT(1)
                    FROM
                        TBL_CONTACTINFO
                    WHERE 1 = 1
                ");
                dynamicParam.Add("o_Cnt", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var sbQuery = new StringBuilder();
                sbQuery.AppendLine(@"
                    OPEN :o_Result FOR
                    SELECT
                        CONTACTINFO_ID AS ContactInfoID,
                        NAME,
                        NICKNAME,
                        GENDER,
                        AGE,
                        PHONE_NO AS PhoneNo,
                        ADDRESS,
                        IS_ENABLE AS IsEnable,
                        CREATE_TIME AS CreateTime,
                        UPDATE_TIME AS UpdateTime
                    FROM
                        TBL_CONTACTINFO
                    WHERE 1 = 1
                ");
                dynamicParam.Add("o_Result", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            sbCnt.AppendLine("AND NAME = :Name");
                            sbQuery.AppendLine("AND NAME = :Name");
                            break;

                        case "Nickname":
                            sbCnt.AppendLine("AND NICKNAME LIKE :Nickname");
                            sbQuery.AppendLine("AND NICKNAME LIKE :Nickname");
                            break;

                        case "Gender":
                            sbCnt.AppendLine("AND GENDER = :Gender");
                            sbQuery.AppendLine("AND GENDER = :Gender");
                            break;
                    }
                    dynamicParam.Add(key, dicParams[key]);
                }
                #endregion

                #region [Order]
                sbQuery.AppendLine("ORDER BY CONTACTINFO_ID DESC");
                #endregion

                #region[Paging]
                sbQuery.AppendLine("OFFSET :RowStart ROWS FETCH NEXT :RowLength ROWS ONLY");
                dynamicParam.Add("RowStart", dicParams["RowStart"]);
                dynamicParam.Add("RowLength", dicParams["RowLength"]);
                #endregion

                var res = _dbConnection.QueryMultiple($"BEGIN {sbCnt}; {sbQuery}; END;", dynamicParam);
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
                sbSQL.AppendLine(@"
                    INSERT INTO TBL_CONTACTINFO
                        (NAME, NICKNAME, GENDER, AGE, PHONE_NO, ADDRESS)
                    VALUES
                        (:Name, :Nickname, :Gender, :Age, :PhoneNo, :Address)
                    RETURNING CONTACTINFO_ID INTO :ContactInfoID
                ");

                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("Name", objContactInfo.Name);
                dynamicParam.Add("Nickname", objContactInfo.Nickname);
                dynamicParam.Add("Gender", (int?)objContactInfo.Gender);
                dynamicParam.Add("Age", objContactInfo.Age);
                dynamicParam.Add("PhoneNo", objContactInfo.PhoneNo);
                dynamicParam.Add("Address", objContactInfo.Address);
                dynamicParam.Add("ContactInfoID", dbType: OracleMappingType.Int64, direction: ParameterDirection.Output);

                _dbConnection.Execute(sbSQL.ToString(), dynamicParam);
                objContactInfo.ContactInfoID = dynamicParam.Get<long?>("ContactInfoID") ?? 0;

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
                sbSQL.AppendLine(@"
                    UPDATE TBL_CONTACTINFO SET
                        NAME = :Name,
                        NICKNAME = :Nickname,
                        GENDER = :Gender,
                        AGE = :Age,
                        PHONE_NO = :PhoneNo,
                        ADDRESS = :Address,
                        UPDATE_TIME = sysdate
                    WHERE
                        CONTACTINFO_ID = :ContactInfoID
                ");

                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.Add("ContactInfoID", objContactInfo.ContactInfoID);
                dynamicParam.Add("Name", objContactInfo.Name);
                dynamicParam.Add("Nickname", objContactInfo.Nickname);
                dynamicParam.Add("Gender", (int?)objContactInfo.Gender);
                dynamicParam.Add("Age", objContactInfo.Age);
                dynamicParam.Add("PhoneNo", objContactInfo.PhoneNo);
                dynamicParam.Add("Address", objContactInfo.Address);

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
                sbSQL.AppendLine(@"
                    DELETE FROM
                        TBL_CONTACTINFO
                    WHERE
                        CONTACTINFO_ID IN :ContactInfoIDs
                ");

                var dynamicParam = new OracleDynamicParameters();
                dynamicParam.AddDynamicParams(new { ContactInfoIDs = ids });

                return _dbConnection.Execute(sbSQL.ToString(), dynamicParam) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Fail : {ex.Message}");
                throw;
            }
        }
    }
}
