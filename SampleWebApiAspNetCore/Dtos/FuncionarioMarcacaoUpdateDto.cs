using System;
using System.ComponentModel.DataAnnotations;

namespace SampleWebApiAspNetCore.Dtos
{
    public class FuncionarioMarcacaoUpdateDto
    {
        public int IdFuncionario { get; set; }
        public int IdMarcacao { get; set; }
    }
}
