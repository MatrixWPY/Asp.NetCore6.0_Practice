using Microsoft.EntityFrameworkCore;
using WebMVC.Models;
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

        public async Task<IEnumerable<IContactInfoModel>> QueryAsync(Dictionary<string, object> dicParams)
        {
            try
            {
                var filter = _dbContext.ContactInfoModels.AsQueryable();

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
                            filter = filter.Where(e => e.Gender == (EnumGender)dicParams[key]);
                            break;
                    }
                }
                #endregion

                #region [Order]
                filter = filter.OrderByDescending(e => e.ContactInfoID);
                #endregion

                return await filter.ToListAsync();
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
                _dbContext.Add((ContactInfoModel)contactInfo);
                await _dbContext.SaveChangesAsync();

                return true;
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
                await _dbContext.SaveChangesAsync();

                return true;
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
