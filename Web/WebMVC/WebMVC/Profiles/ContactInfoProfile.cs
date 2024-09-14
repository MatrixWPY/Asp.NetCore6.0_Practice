using AutoMapper;
using WebMVC.Models.Common;
using WebMVC.Models.Interface;
using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Profiles
{
    public class ContactInfoProfile : Profile
    {
        public ContactInfoProfile()
        {
            CreateMap<IContactInfoModel, QueryRes>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (short?)src.Gender));

            CreateMap<CreateReq, IContactInfoModel>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (EnumGender?)src.Gender));

            CreateMap<EditReq, IContactInfoModel>()
                .ForMember(dest => dest.ContactInfoID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (EnumGender?)src.Gender))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
