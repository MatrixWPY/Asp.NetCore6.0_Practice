﻿using WebMVC.Models;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;
using WebMVC.Services.Interface;
using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Services.Instance
{
    public class ContactInfoService : IContactInfoService
    {
        private IContactInfoModel _contactInfoModel;
        private IContactInfoRepository _contactInfoRepository;

        public ContactInfoService(
            IContactInfoModel contactInfoModel,
            IContactInfoRepository contactInfoRepository
        )
        {
            _contactInfoModel = contactInfoModel;
            _contactInfoRepository = contactInfoRepository;
        }

        public async Task<QueryRes> QueryByIdAsync(long id)
        {
            var res = await _contactInfoRepository.QueryAsync(id);

            return new QueryRes
            {
                ContactInfoID = res.ContactInfoID,
                Name = res.Name,
                Nickname = res.Nickname,
                Gender = (short?)res.Gender,
                Age = res.Age,
                PhoneNo = res.PhoneNo,
                Address = res.Address
            };
        }

        public async Task<IEnumerable<QueryRes>> QueryByConditionAsync(QueryReq req)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(req.Name) == false)
            {
                dicParams["Name"] = req.Name;
            }
            if (string.IsNullOrWhiteSpace(req.Nickname) == false)
            {
                dicParams["Nickname"] = $"%{req.Nickname}%";
            }
            if (req.Gender.HasValue)
            {
                dicParams["Gender"] = req.Gender;
            }

            var res = await _contactInfoRepository.QueryAsync(dicParams);

            return res.Select(e => new QueryRes
            {
                ContactInfoID = e.ContactInfoID,
                Name = e.Name,
                Nickname = e.Nickname,
                Gender = (short?)e.Gender,
                Age = e.Age,
                PhoneNo = e.PhoneNo,
                Address = e.Address
            });
        }

        public async Task<bool> CreateAsync(CreateReq req)
        {
            _contactInfoModel.Name = req.Name;
            _contactInfoModel.Nickname = req.Nickname;
            _contactInfoModel.Gender = (EnumGender?)req.Gender;
            _contactInfoModel.Age = req.Age;
            _contactInfoModel.PhoneNo = req.PhoneNo;
            _contactInfoModel.Address = req.Address;

            return await _contactInfoRepository.InsertAsync(_contactInfoModel);
        }

        public async Task<bool> EditAsync(EditReq req)
        {
            var origin = await _contactInfoRepository.QueryAsync(req.Id ?? 0);
            if (origin == null)
            {
                return false;
            }

            _contactInfoModel.ContactInfoID = req.Id ?? 0;
            _contactInfoModel.Name = string.IsNullOrWhiteSpace(req.Name) ? origin.Name : req.Name;
            _contactInfoModel.Nickname = string.IsNullOrWhiteSpace(req.Nickname) ? origin.Nickname : req.Nickname;
            _contactInfoModel.Gender = (EnumGender?)req.Gender ?? origin.Gender;
            _contactInfoModel.Age = req.Age ?? origin.Age;
            _contactInfoModel.PhoneNo = string.IsNullOrWhiteSpace(req.PhoneNo) ? origin.PhoneNo : req.PhoneNo;
            _contactInfoModel.Address = string.IsNullOrWhiteSpace(req.Address) ? origin.Address : req.Address;

            return await _contactInfoRepository.UpdateAsync(_contactInfoModel);
        }

        public async Task<bool> RemoveAsync(IEnumerable<long> ids)
        {
            return await _contactInfoRepository.DeleteAsync(ids);
        }
    }
}
