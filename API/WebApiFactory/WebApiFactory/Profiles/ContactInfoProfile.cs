using AutoMapper;
using WebApiFactory.DtoModels.ContactInfo;
using WebApiFactory.Models;
using static WebApiFactory.Models.ContactInfo;

namespace WebApiFactory.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class ContactInfoProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public ContactInfoProfile()
        {
            CreateMap<ContactInfo, QueryRP>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (short?)src.Gender));

            CreateMap<CreateRQ, ContactInfo>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (EnumGender?)src.Gender));

            CreateMap<EditRQ, ContactInfo>()
                .ForMember(dest => dest.ContactInfoID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (EnumGender?)src.Gender));

            CreateMap<EditPartialRQ, ContactInfo>()
                .ForMember(dest => dest.ContactInfoID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (EnumGender?)src.Gender))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
