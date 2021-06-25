using SampleWebApiAspNetCore.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SampleWebApiAspNetCore.Services
{
    public class SeedDataService : ISeedDataService
    {
        public async Task Initialize(testePAPContext context)
        {
            //context.Funcao.Add(new FoodEntity() { Calories = 1000, Type = "Starter", Name = "Lasagne", Created = DateTime.Now });
            //context.FoodItems.Add(new FoodEntity() { Calories = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.Now });
            //context.FoodItems.Add(new FoodEntity() { Calories = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.Now });
            //context.FoodItems.Add(new FoodEntity() { Calories = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.Now });

            if (!context.Utilizador.Any())
            {
                // User
                // Falta a gerar a hash ... https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/
                context.Funcao.Add(new Funcao() { Descricao = "Médico" });
                context.Funcao.Add(new Funcao() { Descricao = "Enfermeiro" });
                context.Funcao.Add(new Funcao() { Descricao = "Administrativo" });
                context.Funcao.Add(new Funcao() { Descricao = "Técnico" });
                context.Funcao.Add(new Funcao() { Descricao = "Admin" });
                context.Utilizador.Add(new Utilizador() { Login = "000000000", Senha = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3" });
                context.Funcionario.Add(new Funcionario() { IdUtilizador=1, Nome="Administrador", Sexo="Masculino ", Telemovel="000000000",
                    Nacionalidade="Portugal", DataNasc=new DateTime(2003,01,24),Email= "noreply.ajudamais@gmail.com",Cc="00000000", 
                    Nif="000000000", Funcao=5});

                await context.SaveChangesAsync();
            }

        }
    }
}
