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
                context.Utilizador.Add(new Utilizador() { Login = "Admin", Senha = "123" });
                await context.SaveChangesAsync();
            }

        }
    }
}
