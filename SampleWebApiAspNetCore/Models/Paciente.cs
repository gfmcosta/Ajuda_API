using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Paciente
    {
        public Paciente()
        {
            Marcacaos = new HashSet<Marcacao>();
        }

        public int IdPaciente { get; set; }
        public int IdUtilizador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telemovel { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string Cc { get; set; }
        public string Nif { get; set; }

        [JsonIgnore]
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<Marcacao> Marcacaos { get; set; }
    }
}
