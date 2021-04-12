using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class FuncionarioMarcacaoMappings : Profile
    {
        public FuncionarioMarcacaoMappings()
        {
            CreateMap<FuncionarioMarcacao, FuncionarioMarcacaoDto>().ReverseMap();
            CreateMap<FuncionarioMarcacao, FuncionarioMarcacaoUpdateDto>().ReverseMap();
            CreateMap<FuncionarioMarcacao, FuncionarioMarcacaoCreateDto>().ReverseMap();
        }
    }
}
