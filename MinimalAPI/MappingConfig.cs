using AutoMapper;
using MinimalAPI.Data.DTO;
using MinimalAPI.Models;
using MinimalAPI.Models.DTO;

namespace MinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
