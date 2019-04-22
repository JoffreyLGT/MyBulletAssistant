﻿using AutoMapper;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            this.CreateMap<User, User>();
            this.CreateMap<Entry, Entry>();
        }
    }
}