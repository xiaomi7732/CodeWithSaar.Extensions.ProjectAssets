using System.Linq;
using System.Text.Json;
using CodeWithSaar.ProjectAssets.Models;
using Xunit;

namespace CodeWithSaar.ProjectAssets.Core.Tests;

public class TargetsSerializationTests
{
    [Fact]
    public void ShouldBeAbleToCorrectDeserializeTargets()
    {
        string serialized=@"
{
  ""version"": 3,
  ""targets"": {
    "".NETStandard,Version=v2.0"": {
      ""Microsoft.NETCore.Platforms/1.1.0"": {
        ""type"": ""package"",
        ""compile"": {
          ""lib/netstandard1.0/_._"": {}
        },
        ""runtime"": {
          ""lib/netstandard1.0/_._"": {}
        }
      },
      ""NETStandard.Library/2.0.3"": {
        ""type"": ""package"",
        ""dependencies"": {
          ""Microsoft.NETCore.Platforms"": ""1.1.0""
        },
        ""compile"": {
          ""lib/netstandard1.0/_._"": {}
        },
        ""runtime"": {
          ""lib/netstandard1.0/_._"": {}
        },
        ""build"": {
          ""build/netstandard2.0/NETStandard.Library.targets"": {}
        }
      }
    }
  }
}
        ";
        Assets? actual = JsonSerializer.Deserialize<Assets>(serialized, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        
        Assert.NotNull(actual);
        Assert.NotNull(actual!.Targets);
        Assert.Equal(1, actual!.Targets!.Count); // .NETStandard,Version=v2.0
        Assert.Equal(".NETStandard,Version=v2.0", actual!.Targets!.First().Key); // .NETStandard,Version=v2.0
        Assert.Equal(2, actual!.Targets!.First().Value.Count);

        AssetPackageInfo info1 = actual!.Targets!.First().Value.First().Value;
        Assert.Equal("package", info1.Type);
        Assert.Null(info1.Dependencies);
        
        AssetPackageInfo info2 = actual!.Targets!.First().Value.Skip(1).First().Value;
        Assert.Equal("package", info2.Type);
        Assert.NotNull(info2.Dependencies);
        Assert.Equal("Microsoft.NETCore.Platforms", info2.Dependencies!.First().Key);
    }
}