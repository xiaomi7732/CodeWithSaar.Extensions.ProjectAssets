using CodeWithSaar.ProjectAssets.Core;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Program p = new Program();

        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
        {
            await Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsedAsync<CmdOptions>(async (options) => await p.StartAsync(options, cancellationTokenSource.Token));
        }
    }

    public async Task StartAsync(CmdOptions options, CancellationToken cancellationToken)
    {
        IConfiguration configuration = BuildConfiguration();
        IServiceCollection services = new ServiceCollection();
        services = RegisterServices(services, configuration, options);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ILogger logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            await StartAsync(serviceProvider.GetRequiredService<Engine>(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
        }
    }

    private Task StartAsync(Engine engine, CancellationToken cancellationToken)
        => engine.RunAsync(cancellationToken);

    private IServiceCollection RegisterServices(IServiceCollection services, IConfiguration configuration, CmdOptions cmdOptions)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddSimpleConsole(opt =>
            {
                opt.SingleLine = true;
            });
            loggingBuilder.AddFile();
        });

        services.TryAddSingleton(cmdOptions);
        services.TryAddTransient<IFileExistCheck, DefaultFileExistCheck>();
        services.TryAddTransient<ILocateAssetJson, TargetFileLocator>();
        services.TryAddTransient<IDeserializeAssets, AssetsDeserializer>();
        services.TryAddTransient<IGenerateVisual<MermaidGenOptions>, MermaidGen>();
        services.TryAddTransient<IManageKnownEdge, KnownEdgeManager>();
        
        services.TryAddTransient<Engine>();
        return services;
    }

    private IConfiguration BuildConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.jsonc", optional: false, reloadOnChange: true)
            .Build();
        return configuration;
    }
}