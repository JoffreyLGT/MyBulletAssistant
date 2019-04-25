using AutoMapper;
using Core.Models;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class MbaMapperProfile : Profile
    {
        public MbaMapperProfile()
        {
            this.CreateMap<User, User>()
                .ForMember(m => m.Id, id => id.Ignore());
            this.CreateMap<Entry, Entry>();
            this.CreateMap<User, UserModel>()
                .ReverseMap();
            this.CreateMap<Entry, EntryModel>();
        }
    }
}
