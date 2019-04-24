using AutoMapper;
using Core.Models;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            this.CreateMap<User, User>();
            this.CreateMap<Entry, Entry>();
            this.CreateMap<User, UserModel>()
                .ReverseMap();
            this.CreateMap<Entry, EntryModel>();
        }
    }
}
