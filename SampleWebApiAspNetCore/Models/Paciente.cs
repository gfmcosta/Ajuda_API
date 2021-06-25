using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Paciente
    {
        
        public Paciente()
        {
            
            Marcacao = new HashSet<Marcacao>();
        }

        public int IdPaciente { get; set; }

        public int? IdUtilizador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telemovel { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string Cc { get; set; }
        public string Nif { get; set; }
        public Boolean Ativo { get; set; }

        [JsonIgnore]
        public virtual Utilizador UtilizadorNavigation { get; set; }
   
        [JsonIgnore]
        public virtual ICollection<Marcacao> Marcacao { get; set; }
    }
}
