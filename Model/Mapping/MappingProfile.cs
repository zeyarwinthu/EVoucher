using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.Model.Entity;
using WebAPI.Model.Request;

namespace WebAPI.Model
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserRequest>().ReverseMap();
            CreateMap<EVoucher, EVoucherRequest>().ReverseMap();
        }
    }
}
