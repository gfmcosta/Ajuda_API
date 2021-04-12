using SampleWebApiAspNetCore.Models;
using System.Threading.Tasks;

namespace SampleWebApiAspNetCore.Services
{
    public interface ISeedDataService
    {
        Task Initialize( testePAPContext context);
    }
}
