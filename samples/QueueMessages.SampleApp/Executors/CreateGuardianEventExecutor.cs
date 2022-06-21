using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Services;
using Microsoft.Extensions.Logging;
using SampleApp.GuardiansOfTheGalaxy.Services;

namespace SampleApp.GuardiansOfTheGalaxy.Executors
{
    internal class CreateGuardianEventExecutor : IQueueMessageExecutor
    {
        private readonly ILogger<CreateGuardianEventExecutor> _logger;
        private readonly IGuardianCreator _guardianCreator;

        public CreateGuardianEventExecutor(ILogger<CreateGuardianEventExecutor> logger, IGuardianCreator guardianCreator)
        {
            _logger = logger;
            _guardianCreator = guardianCreator;
        }

        public async Task Execute(QueueMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event '{MessageId}' retrieved, creating guardian", message.MessageId);

            var guardian = await _guardianCreator.Create(cancellationToken);

            _logger.LogInformation("Guardian Created! \n Name: {Name}, \n Weapon: {Weapon}, \n Damage: {Damage}",
                guardian.Name,
                guardian.Weapon.Name,
                guardian.Weapon.Damage);
        }
    }
}
