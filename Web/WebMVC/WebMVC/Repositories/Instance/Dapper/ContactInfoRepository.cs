using Dapper;
using System.Data;
using System.Text;
using WebMVC.Models.Instance.Dapper;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.Dapper
{
    public class ContactInfoRepository : IContactInfoRepository
    {
        private IDbConnection _dbConnection;

        public ContactInfoRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IContactInfoModel?> QueryAsync(long id)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine(@"
                    SELECT
                        *
                    FROM
                        dbo.Tbl_ContactInfo
                    WHERE
                        ContactInfoID = @ContactInfoID
                ");

                return await _dbConnection.QueryFirstOrDefaultAsync<ContactInfoModel>(sbSQL.ToString(), new { ContactInfoID = id });
            }
            catch
            {
                throw;
            }
        }

        public async Task<(int totalCnt,IEnumerable<IContactInfoModel> data)> QueryAsync(Dictionary<string, object> dicParams)
        {
            try
            {
                var sbCnt = new StringBuilder();
                sbCnt.AppendLine(@"
                    SELECT
                        COUNT(1)
                    FROM
                        dbo.Tbl_ContactInfo
                    WHERE
                        1=1
                ");
                var sbData = new StringBuilder();
                sbData.AppendLine(@"
                    SELECT
                        *
                    FROM
                        dbo.Tbl_ContactInfo
                    WHERE
                        1=1
                ");

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            sbCnt.AppendLine("AND Name = @Name");
                            sbData.AppendLine("AND Name = @Name");
                            break;

                        case "Nickname":
                            sbCnt.AppendLine("AND Nickname LIKE @Nickname");
                            sbData.AppendLine("AND Nickname LIKE @Nickname");
                            dicParams[key] = $"%{dicParams[key]}%";
                            break;

                        case "Gender":
                            sbCnt.AppendLine("AND Gender = @Gender");
                            sbData.AppendLine("AND Gender = @Gender");
                            break;
                    }
                }
                #endregion

                #region [Order]
                sbData.AppendLine("ORDER BY ContactInfoID DESC");
                #endregion

                #region [Paging]
                sbData.AppendLine("OFFSET @RowStart ROWS FETCH NEXT @RowLength ROWS ONLY");
                #endregion

                var res = await _dbConnection.QueryMultipleAsync(sbCnt.ToString() + sbData.ToString(), dicParams);
                return (res.Read<int>().FirstOrDefault(), res.Read<ContactInfoModel>());
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> InsertAsync(IContactInfoModel contactInfo)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine(@"
                    INSERT INTO dbo.Tbl_ContactInfo
                        (Name, Nickname, Gender, Age, PhoneNo, Address, CreateTime)
                    VALUES
                        (@Name, @Nickname, @Gender, @Age, @PhoneNo, @Address, @CreateTime)
                    SELECT SCOPE_IDENTITY()
                ");

                contactInfo.ContactInfoID = await _dbConnection.ExecuteScalarAsync<long?>(sbSQL.ToString(), contactInfo) ?? 0;
                return contactInfo.ContactInfoID > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(IContactInfoModel contactInfo)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine(@"
                    UPDATE dbo.Tbl_ContactInfo SET
                        Name = @Name, Nickname = @Nickname, Gender = @Gender, Age = @Age, PhoneNo = @PhoneNo, Address = @Address, UpdateTime = @UpdateTime
                    WHERE
                        ContactInfoID = @ContactInfoID
                ");

                return await _dbConnection.ExecuteAsync(sbSQL.ToString(), contactInfo) > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IEnumerable<long> ids)
        {
            try
            {
                var sbSQL = new StringBuilder();
                sbSQL.AppendLine(@"
                    DELETE FROM
                        dbo.Tbl_ContactInfo
                    WHERE
                        ContactInfoID IN @ContactInfoIDs
                ");

                return await _dbConnection.ExecuteAsync(sbSQL.ToString(), new { ContactInfoIDs = ids }) > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
