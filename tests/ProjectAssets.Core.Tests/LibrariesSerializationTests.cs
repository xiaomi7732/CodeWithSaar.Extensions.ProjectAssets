using System.Linq;
using System.Text.Json;
using CodeWithSaar.ProjectAssets.Models;
using Xunit;

namespace CodeWithSaar.ProjectAssets.Core.Tests;

public class LibrariesSerializationTests
{
    [Fact]
    public void ShouldBeAbleToCorrectDeserializeTargets()
    {
        string serialized=@"
{
  ""version"": 3,
  ""libraries"": {
    ""Microsoft.NETCore.Platforms/1.1.0"": {
      ""sha512"": ""kz0PEW2lhqygehI/d6XsPCQzD7ff7gUJaVGPVETX611eadGsA3A877GdSlU0LRVMCTH/+P3o2iDTak+S08V2+A=="",
      ""type"": ""package"",
      ""path"": ""microsoft.netcore.platforms/1.1.0"",
      ""files"": [
        "".nupkg.metadata"",
        "".signature.p7s"",
        ""ThirdPartyNotices.txt"",
        ""dotnet_library_license.txt"",
        ""lib/netstandard1.0/_._"",
        ""microsoft.netcore.platforms.1.1.0.nupkg.sha512"",
        ""microsoft.netcore.platforms.nuspec"",
        ""runtime.json""
      ]
    }
  }
}
        ";
        Assets? actual = JsonSerializer.Deserialize<Assets>(serialized, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        
        Assert.NotNull(actual);
        Assert.NotNull(actual!.Libraries);
        Assert.Equal(1, actual!.Libraries!.Count);
        
        Assert.Equal("Microsoft.NETCore.Platforms/1.1.0", actual!.Libraries!.First().Key); // .NETStandard,Version=v2.0
        AssetLibraryInfo libraryInfo = actual!.Libraries.First().Value;
        Assert.NotNull(libraryInfo);
        Assert.Equal("kz0PEW2lhqygehI/d6XsPCQzD7ff7gUJaVGPVETX611eadGsA3A877GdSlU0LRVMCTH/+P3o2iDTak+S08V2+A==", libraryInfo.Sha512);
        Assert.Equal(8, libraryInfo.Files.Count());
    }
}