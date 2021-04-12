using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class FuncaoMappings : Profile
    {
        public FuncaoMappings()
        {
            CreateMap<Funcao, FuncaoDto>().ReverseMap();
            CreateMap<Funcao, FuncaoUpdateDto>().ReverseMap();
            CreateMap<Funcao, FuncaoCreateDto>().ReverseMap();
        }
    }
}
