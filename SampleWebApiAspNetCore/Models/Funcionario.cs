using System;
using System.Collections.Generic;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Funcionario
    {
        public Funcionario()
        {
            FuncionarioMarcacaos = new HashSet<FuncionarioMarcacao>();
        }

        public int IdFuncionario { get; set; }
        public int IdUtilizador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telemovel { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string Cc { get; set; }
        public string Nif { get; set; }
        public int Funcao { get; set; }

        public virtual Funcao FuncaoNavigation { get; set; }
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
        public virtual ICollection<FuncionarioMarcacao> FuncionarioMarcacaos { get; set; }
    }
}
