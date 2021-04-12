using System;

namespace SampleWebApiAspNetCore.Dtos
{
    public class FuncionarioMarcacaoCreateDto
    {
        public int IdFuncionarioMarcacao { get; set; }
        public int IdFuncionario { get; set; }
        public int IdMarcacao { get; set; }
    }
}
