using SqlSugar;
using WebMVC.Models.Instance.SqlSugar;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.SqlSugar
{
    public class ContactInfoRepository : IContactInfoRepository
    {
        private ISqlSugarClient _client;

        public ContactInfoRepository(ISqlSugarClient client)
        {
            _client = client;
        }

        public async Task<IContactInfoModel?> QueryAsync(long id)
        {
            try
            {
                return await _client.Queryable<ContactInfoModel>().InSingleAsync(id);
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
                var filter = _client.Queryable<ContactInfoModel>();

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            filter = filter.Where(e => e.Name == (string)dicParams[key]);
                            break;

                        case "Nickname":
                            filter = filter.Where(e => e.Nickname.Contains((string)dicParams[key]));
                            break;

                        case "Gender":
                            filter = filter.Where(e => (short)e.Gender == (short)dicParams[key]);
                            break;
                    }
                }
                #endregion

                #region [Order]
                filter = filter.OrderByDescending(e => e.ContactInfoID);
                #endregion

                #region [Paging]
                var pageNum = ((int)dicParams.GetValueOrDefault("RowStart") / (int)dicParams.GetValueOrDefault("RowLength")) + 1;
                var pageSize = (int)dicParams.GetValueOrDefault("RowLength");
                #endregion

                RefAsync<int> totalCnt = 0;
                var data = await filter.ToPageListAsync(pageNum, pageSize, totalCnt);

                return (totalCnt, data);
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
                contactInfo.ContactInfoID = await _client.Insertable<ContactInfoModel>(contactInfo).ExecuteReturnIdentityAsync();
                return contactInfo.ContactInfoID > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<(bool result, string errorMsg)> UpdateAsync(IContactInfoModel contactInfo)
        {
            try
            {
                var res = await _client.Updateable<ContactInfoModel>(contactInfo)
                            .Where(e => e.ContactInfoID == contactInfo.ContactInfoID && e.RowVersion == contactInfo.RowVersion)
                            .ExecuteCommandAsync();
                return res == 0 ? (false, "資料已被修改") : (true, string.Empty);
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
                return await _client.Deleteable<ContactInfoModel>().In(e => e.ContactInfoID, ids).ExecuteCommandAsync() > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
