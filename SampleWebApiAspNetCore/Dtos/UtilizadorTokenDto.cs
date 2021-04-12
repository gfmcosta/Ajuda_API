using System;

namespace SampleWebApiAspNetCore.Dtos
{
    public class UtilizadorTokenDto
    {
        public int IdUtilizador { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; }

        public string Token { get; set; }

    }
}
