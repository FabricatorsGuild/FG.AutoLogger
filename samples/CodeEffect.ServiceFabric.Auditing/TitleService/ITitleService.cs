using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace TitleService
{
    public interface ITitleService : IService
    {
        Task<string[]> GetTitlesAsync(CancellationToken cancellationToken);

        Task UpdateTitleAsync(string person, string title, CancellationToken cancellationToken);

        Task<string[]> GetPersonsWithTitleAsync(string title, CancellationToken cancellationToken);
    }
}