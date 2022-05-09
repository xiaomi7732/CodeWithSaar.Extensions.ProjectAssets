using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeWithSaar.ProjectAssets.CLI;

public static class AssetJsonFileLocatorRegister
{
    public static IServiceCollection TryAddAssetJsonFileLocators(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<ILocateAssetJson, TargetFileLocator>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<ILocateAssetJson, ObjFolderSearcher>());

        services.TryAddTransient<IAssetFileProvider, AssetFileProvider>();

        return services;
    }
}