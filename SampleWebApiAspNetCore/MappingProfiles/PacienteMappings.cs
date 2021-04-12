using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class PacienteMappings : Profile
    {
        public PacienteMappings()
        {
            CreateMap<Paciente, PacienteDto>().ReverseMap();
            CreateMap<Paciente, PacienteUpdateDto>().ReverseMap();
            CreateMap<Paciente, PacienteCreateDto>().ReverseMap();
        }
    }
}
