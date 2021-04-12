using System;

namespace SampleWebApiAspNetCore.Dtos
{
    public class MarcacaoDto
    {
        public int IdMarcacao { get; set; }
        public int IdPaciente { get; set; }
        public DateTime Data { get; set; }
        public DateTime Hora { get; set; }
        public string Tipo { get; set; }
        public string QRCODE { get; set; }
        public string Relatorio { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
