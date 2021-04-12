using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class FuncionarioMappings : Profile
    {
        public FuncionarioMappings()
        {
            CreateMap<Funcionario, FuncionarioDto>().ReverseMap();
            CreateMap<Funcionario, FuncionarioUpdateDto>().ReverseMap();
            CreateMap<Funcionario, FuncionarioCreateDto>().ReverseMap();
        }
    }
}
