using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Funcionario
    {
        public int IdFuncionario { get; set; }
        public int? IdUtilizador { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telemovel { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string Cc { get; set; }
        public string Nif { get; set; }
        public int Funcao { get; set; }
        public Boolean Ativo { get; set; }
        [NotMapped]
        public virtual Funcao FuncaoNavigation { get; set; }
        [NotMapped]
        public virtual Utilizador UtilizadorNavigation { get; set; }
       
        [JsonIgnore]
        public virtual ICollection<Marcacao> Marcacao { get; set; }
    }
}
