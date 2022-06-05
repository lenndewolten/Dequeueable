using Microsoft.Extensions.Hosting;
using SampleApp.GuardiansOfTheGalaxy;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.ConfigureDefaultServices();
    })
    .RunConsoleAsync();