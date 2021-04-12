using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Utilizador
    {
        
        public int IdUtilizador { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }

        public virtual ICollection<Funcionario> Funcionarios { get; set; }
        public virtual ICollection<Paciente> Pacientes { get; set; }

        [NotMapped]
        public string Perfil
        {
            get
            {
                return ((Pacientes != null) ? "Paciente" : (Funcionarios == null) ? "NãoAtribuido" : Funcionarios.First().FuncaoNavigation.Descricao);
            }
        }

    }
}
