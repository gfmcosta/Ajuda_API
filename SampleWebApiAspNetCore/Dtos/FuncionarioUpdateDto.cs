using System;
using System.ComponentModel.DataAnnotations;

namespace SampleWebApiAspNetCore.Dtos
{
    public class FuncionarioUpdateDto
    {
        public int IdUtilizador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telemovel { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string CC { get; set; }
        public string NIF { get; set; }
        public int Funcao { get; set; }
        public Boolean Ativo { get; set; }

    }
}
