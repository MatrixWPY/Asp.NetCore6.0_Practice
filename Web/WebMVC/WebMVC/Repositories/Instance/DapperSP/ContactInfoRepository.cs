using Dapper;
using System.Data;
using WebMVC.Models.Instance.DapperSP;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;

namespace WebMVC.Repositories.Instance.DapperSP
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
                return await _dbConnection.QueryFirstOrDefaultAsync<ContactInfoModel>(
                    "dbo.Sp_GetContactInfo",
                    new { ContactInfoID = id },
                    commandType: CommandType.StoredProcedure
                );
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
                var res = await _dbConnection.QueryMultipleAsync(
                    $"dbo.Sp_ListContactInfo",
                    new
                    {
                        Name = dicParams.GetValueOrDefault("Name"),
                        Nickname = dicParams.GetValueOrDefault("Nickname"),
                        Gender = dicParams.GetValueOrDefault("Gender"),
                        RowStart = dicParams.GetValueOrDefault("RowStart"),
                        RowLength = dicParams.GetValueOrDefault("RowLength"),
                    },
                    commandType: CommandType.StoredProcedure
                );
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
                contactInfo.ContactInfoID = await _dbConnection.ExecuteScalarAsync<long?>(
                    "dbo.Sp_AddContactInfo",
                    new
                    {
                        contactInfo.Name,
                        contactInfo.Nickname,
                        contactInfo.Gender,
                        contactInfo.Age,
                        contactInfo.PhoneNo,
                        contactInfo.Address,
                        contactInfo.CreateTime
                    },
                    commandType: CommandType.StoredProcedure
                ) ?? 0;
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
                return await _dbConnection.ExecuteAsync(
                    "dbo.Sp_EditContactInfo",
                    new
                    {
                        contactInfo.ContactInfoID,
                        contactInfo.Name,
                        contactInfo.Nickname,
                        contactInfo.Gender,
                        contactInfo.Age,
                        contactInfo.PhoneNo,
                        contactInfo.Address,
                        contactInfo.UpdateTime
                    },
                    commandType: CommandType.StoredProcedure
                ) > 0;
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
                return await _dbConnection.ExecuteAsync(
                    "dbo.Sp_RemoveContactInfo",
                    new { ContactInfoIDs = string.Join(",", ids) },
                    commandType: CommandType.StoredProcedure
                ) > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
