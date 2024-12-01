using PetaPoco;
using WebMVC.Models.Instance.PetaPoco;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.PetaPoco
{
    public class ContactInfoRepository : IContactInfoRepository
    {
        private readonly Database _db;

        public ContactInfoRepository(Database db)
        {
            _db = db;
        }

        public async Task<IContactInfoModel?> QueryAsync(long id)
        {
            try
            {
                return await _db.SingleOrDefaultAsync<ContactInfoModel>("WHERE ContactInfoID = @0", id);
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
                var sql = new Sql("SELECT * FROM dbo.Tbl_ContactInfo WHERE 1=1");

                #region [Query Condition]
                foreach (var key in dicParams.Keys)
                {
                    switch (key)
                    {
                        case "Name":
                            sql.Append("AND Name = @Name", new { Name = dicParams[key] });
                            break;

                        case "Nickname":
                            sql.Append("AND Nickname LIKE @Nickname", new { Nickname = $"%{dicParams[key]}%" });
                            break;

                        case "Gender":
                            sql.Append("AND Gender = @Gender", new { Gender = dicParams[key] });
                            break;
                    }
                }
                #endregion

                #region [Order]
                sql.Append("ORDER BY ContactInfoID DESC");
                #endregion

                #region [Paging]
                var pageNum = ((int)dicParams.GetValueOrDefault("RowStart") / (int)dicParams.GetValueOrDefault("RowLength")) + 1;
                var pageSize = (int)dicParams.GetValueOrDefault("RowLength");
                #endregion

                var res = await _db.PageAsync<ContactInfoModel>(pageNum, pageSize, sql);

                return ((int)res.TotalItems, res.Items);
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
                contactInfo.ContactInfoID = (long?)await _db.InsertAsync(contactInfo) ?? 0;
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
                var res = await _db.UpdateAsync<ContactInfoModel>(@"
                    SET
                        Name = @Name, Nickname = @Nickname, Gender = @Gender, Age = @Age, PhoneNo = @PhoneNo, Address = @Address, UpdateTime = @UpdateTime
                    WHERE
                        ContactInfoID = @ContactInfoID
                    AND
                        RowVersion = @RowVersion",
                    contactInfo
                );
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
                return await _db.DeleteAsync<ContactInfoModel>("WHERE ContactInfoID IN (@0)", ids) > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
