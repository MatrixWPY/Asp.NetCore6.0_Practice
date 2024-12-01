using Microsoft.EntityFrameworkCore;
using WebMVC.Models.Instance.EFCore;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.EFCore
{
    public class ContactInfoRepository : IContactInfoRepository
    {
        private WebMvcDbContext _dbContext;

        public ContactInfoRepository(WebMvcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IContactInfoModel?> QueryAsync(long id)
        {
            try
            {
                return await _dbContext.ContactInfoModels.FindAsync(id);
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
                var filterCnt = _dbContext.ContactInfoModels.AsQueryable();
                var filterData = _dbContext.ContactInfoModels.AsQueryable();

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            filterCnt = filterCnt.Where(e => e.Name == (string)dicParams[key]);
                            filterData = filterData.Where(e => e.Name == (string)dicParams[key]);
                            break;

                        case "Nickname":
                            filterCnt = filterCnt.Where(e => e.Nickname.Contains((string)dicParams[key]));
                            filterData = filterData.Where(e => e.Nickname.Contains((string)dicParams[key]));
                            break;

                        case "Gender":
                            filterCnt = filterCnt.Where(e => (short)e.Gender == (short)dicParams[key]);
                            filterData = filterData.Where(e => (short)e.Gender == (short)dicParams[key]);
                            break;
                    }
                }
                #endregion

                #region [Order]
                filterData = filterData.OrderByDescending(e => e.ContactInfoID);
                #endregion

                #region [Paging]
                filterData = filterData.Skip((int)dicParams["RowStart"]).Take((int)dicParams["RowLength"]);
                #endregion

                return (await filterCnt.CountAsync(), await filterData.ToListAsync());
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
                _dbContext.Add(contactInfo);
                await _dbContext.SaveChangesAsync();

                return true;
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
                // EFCore中樂觀併發控制依賴於 OriginalValue 進行版本檢查
                // 顯式手動設置 OriginalValue = CurrentValue
                _dbContext.Entry(contactInfo).Property(e => e.RowVersion).OriginalValue = contactInfo.RowVersion;
                _dbContext.Update(contactInfo);
                await _dbContext.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "資料已被修改");
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
                var contactInfos = await _dbContext.ContactInfoModels.Where(e => ids.Contains(e.ContactInfoID)).ToListAsync();
                if (contactInfos.Any())
                {
                    _dbContext.ContactInfoModels.RemoveRange(contactInfos);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}
