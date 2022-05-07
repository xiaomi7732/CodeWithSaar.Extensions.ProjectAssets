using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.Core;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AssetsExtensions
{
    /// <summary>
    /// Gets the type of the library.
    /// </summary>
    /// <param name="assets">The assets.</param>
    /// <param name="packageSignature">A package name / package version string. Like "Microsoft.Extensions.Logging.Console/6.0.0".</param>
    /// <returns></returns>
    public static string GetLibraryType(this Assets assets, string packageSignature)
    {
        if (string.IsNullOrEmpty(packageSignature))
        {
            return string.Empty;
        }

        if (assets.Libraries is null)
        {
            throw new InvalidOperationException("No libraries specified in assets.");
        }

        string? hitKey = assets.Libraries.Keys.FirstOrDefault(k => string.Equals(k, packageSignature, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(hitKey))
        {
            return string.Empty;
        }
        string? type = assets.Libraries[hitKey].Type;
        if (string.IsNullOrEmpty(type))
        {
            return string.Empty;
        }
        return type!;
    }

    public static (string packageSignature, AssetPackageInfo libraryInfo) TryFindTargetLibrary(this Assets assets, string frameworkName, string packageName)
    {
        IDictionary<string, AssetPackageInfo> libraryInfo = FindTargetLibraryInfo(assets, frameworkName, packageName);
        KeyValuePair<string, AssetPackageInfo> hit = libraryInfo.FirstOrDefault(pair => pair.Key.StartsWith(packageName + '/', StringComparison.OrdinalIgnoreCase));
        return (hit.Key, hit.Value);
    }

    public static AssetPackageInfo? TryFindTargetLibrary(this Assets assets, string frameworkName, string packageName, string packageVersion)
    {
        IDictionary<string, AssetPackageInfo> packageInfos = FindTargetLibraryInfo(assets, frameworkName, packageName);

        string packageSignature = packageName + "/" + packageVersion;
        if (packageInfos.ContainsKey(packageSignature))
        {
            return packageInfos[packageSignature];
        }
        return null;
    }

    public static IEnumerable<string> TryGetTargetFrameworks(this Assets assets)
    {
        if (assets is null)
        {
            throw new ArgumentNullException(nameof(assets));
        }

        if (assets.Targets is null)
        {
            throw new InvalidOperationException("The given assets doesn't have any targets.");
        }

        foreach (string frameworkName in assets.Targets.Keys)
        {
            yield return frameworkName;
        }
    }

    private static IDictionary<string, AssetPackageInfo> FindTargetLibraryInfo(Assets assets, string frameworkName, string packageName)
    {
        if (assets is null)
        {
            throw new ArgumentNullException(nameof(assets));
        }

        if (assets.Targets is null)
        {
            throw new InvalidOperationException("No libraries specified in assets.");
        }

        if (string.IsNullOrEmpty(frameworkName))
        {
            throw new ArgumentException($"'{nameof(frameworkName)}' cannot be null or empty.", nameof(frameworkName));
        }

        if (string.IsNullOrEmpty(packageName))
        {
            throw new ArgumentException($"'{nameof(packageName)}' cannot be null or empty.", nameof(packageName));
        }

        if (!assets.Targets.ContainsKey(frameworkName))
        {
            throw new InvalidOperationException($"Framework name can't be found. {frameworkName}");
        }

        return assets.Targets[frameworkName];
    }
}