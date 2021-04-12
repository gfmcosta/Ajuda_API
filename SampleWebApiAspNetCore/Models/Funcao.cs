using System;
using System.Collections.Generic;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Funcao
    {
        public Funcao()
        {
            Funcionarios = new HashSet<Funcionario>();
        }

        public int IdFuncao { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<Funcionario> Funcionarios { get; set; }
    }
}
