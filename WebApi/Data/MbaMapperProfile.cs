using AutoMapper;
using Core.Models;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class MbaMapperProfile : Profile
    {
        public MbaMapperProfile()
        {
            CreateMap<User, User>()
                .ForMember(u => u.Id, id => id.Ignore());

            CreateMap<Entry, Entry>()
                .ForMember(e => e.Id, id => id.Ignore());

            CreateMap<User, UserModel>()
                .ReverseMap();

            CreateMap<Entry, EntryModel>()
                .ReverseMap();

            CreateMap<User, LoginModel>()
                .ForMember(lm=>lm.Password, o=>o.MapFrom(u=>u.PasswordHash))
                .ReverseMap();

        }
    }
}
