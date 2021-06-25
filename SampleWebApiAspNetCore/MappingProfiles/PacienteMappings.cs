using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;
using System.Linq;

namespace SampleWebApiAspNetCore.MappingProfiles
{

    public static class IgnoreVirtualExtensions
    {
        public static IMappingExpression<TSource, TDestination>
               IgnoreAllVirtual<TSource, TDestination>(
                   this IMappingExpression<TSource, TDestination> expression)
        {
            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties().Where(p =>
                                     p.GetGetMethod().IsVirtual))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }
    }

    public class PacienteMappings : Profile
    {
        public PacienteMappings()
        {
            CreateMap<Paciente, PacienteDto>()
                .IgnoreAllVirtual()
                .ReverseMap();
            CreateMap<Paciente, PacienteUpdateDto>()
                .IgnoreAllVirtual()
                .ReverseMap();
            CreateMap<Paciente, PacienteCreateDto>()
                //.ForMember(dest => dest.Senha, opt => opt.MapFrom(s => s.UtilizadorNavigation.Senha))
                .ForMember(dest => dest.Senha, opt => opt.Ignore())
                .IgnoreAllVirtual()
                .ReverseMap();
        }
    }
}
