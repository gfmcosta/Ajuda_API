using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AutoMapper.Configuration.Annotations;

namespace SampleWebApiAspNetCore.Models
{
    public partial class Utilizador
    {
        
        public int IdUtilizador { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
      
        [Ignore]
        public virtual Funcionario Funcionario { get; set; }
        [Ignore]
        public virtual Paciente Paciente { get; set; }

        [Ignore]
        [NotMapped]
        public string Perfil
        {
            get
            {
                return ((Paciente != null) ? "Paciente" : (Funcionario == null) ? "NãoAtribuido" : Funcionario.FuncaoNavigation.Descricao);
            }
        }
        
    }
}
