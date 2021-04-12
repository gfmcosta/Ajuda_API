using System;
using System.Collections.Generic;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class Marcacao
    {
        public Marcacao()
        {
            FuncionarioMarcacaos = new HashSet<FuncionarioMarcacao>();
        }

        public int IdMarcacao { get; set; }
        public int IdPaciente { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Tipo { get; set; }
        public string Qrcode { get; set; }
        public string Relatorio { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        public virtual Paciente IdPacienteNavigation { get; set; }
        public virtual ICollection<FuncionarioMarcacao> FuncionarioMarcacaos { get; set; }
    }
}
