using System;
using System.Collections.Generic;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class FuncionarioMarcacao
    {
        public int IdFuncionarioMarcacao { get; set; }
        public int IdFuncionario { get; set; }
        public int IdMarcacao { get; set; }

        public virtual Funcionario IdFuncionarioNavigation { get; set; }
        public virtual Marcacao IdMarcacaoNavigation { get; set; }
    }
}
