using System;
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
}