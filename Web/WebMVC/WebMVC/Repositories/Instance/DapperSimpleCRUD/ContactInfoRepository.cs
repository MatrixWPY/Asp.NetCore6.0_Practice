using Dapper;
using System.Data;
using System.Text;
using WebMVC.Models.Instance.DapperSimpleCRUD;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.DapperSimpleCRUD
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
                return await _dbConnection.GetAsync<ContactInfoModel>(id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<(int totalCnt, IEnumerable<IContactInfoModel> data)> QueryAsync(Dictionary<string, object> dicParams)
        {
            try
            {
                #region [Query Condition]
                var sbCondition = new StringBuilder("WHERE 1=1");
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            sbCondition.AppendLine("AND Name = @Name");
                            break;

                        case "Nickname":
                            sbCondition.AppendLine("AND Nickname LIKE @Nickname");
                            dicParams[key] = $"%{dicParams[key]}%";
                            break;

                        case "Gender":
                            sbCondition.AppendLine("AND Gender = @Gender");
                            break;
                    }
                }
                #endregion

                #region [Paging]
                var pageNum = ((int)dicParams.GetValueOrDefault("RowStart") / (int)dicParams.GetValueOrDefault("RowLength")) + 1;
                var pageSize = (int)dicParams.GetValueOrDefault("RowLength");
                #endregion

                var cnt = await _dbConnection.RecordCountAsync<ContactInfoModel>(
                    sbCondition.ToString(),
                    dicParams
                );
                var res = await _dbConnection.GetListPagedAsync<ContactInfoModel>(
                    pageNum,
                    pageSize,
                    sbCondition.ToString(),
                    "ContactInfoID DESC",
                    dicParams
                );

                return (cnt, res);
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
                contactInfo.ContactInfoID = (long)await _dbConnection.InsertAsync(contactInfo);
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
                return await _dbConnection.UpdateAsync(contactInfo) > 0;
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
                return await _dbConnection.DeleteListAsync<ContactInfoModel>($"WHERE ContactInfoID IN ({string.Join(",", ids)})") > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
