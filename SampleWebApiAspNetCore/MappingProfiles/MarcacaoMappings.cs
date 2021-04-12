using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class MarcacaoMappings : Profile
    {
        public MarcacaoMappings()
        {
            CreateMap<Marcacao, MarcacaoDto>().ReverseMap();
            CreateMap<Marcacao, MarcacaoUpdateDto>().ReverseMap();
            CreateMap<Marcacao, MarcacaoCreateDto>().ReverseMap();
        }
    }
}
