using SampleApp.GuardiansOfTheGalaxy.Models;

namespace SampleApp.GuardiansOfTheGalaxy.Services
{
    internal interface IGuardianCreator
    {
        Task<Guardian> Create(CancellationToken cancellationToken);
    }
}