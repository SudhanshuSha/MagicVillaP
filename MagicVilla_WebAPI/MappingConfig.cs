using AutoMapper;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;

namespace MagicVilla_WebAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // as long as the name of the variable is same it will auto map it 
            // we can also do custom mapping
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<ApplicationUser, LoginResponseDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

        }
    }
}
