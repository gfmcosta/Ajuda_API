using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class UtilizadorMappings : Profile
    {
        public UtilizadorMappings()
        {
            CreateMap<Utilizador, UtilizadorDto>().ReverseMap();
            CreateMap<Utilizador, UtilizadorUpdateDto>().ReverseMap();
            CreateMap<Utilizador, UtilizadorCreateDto>().ReverseMap();
        }
    }
}
