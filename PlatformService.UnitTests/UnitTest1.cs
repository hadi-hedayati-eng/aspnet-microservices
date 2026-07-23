using FluentAssertions;
using PlatformService.Contracts;

namespace PlatformService.UnitTests;

public class UnitTest1
{
    [Fact]
    public void This_Should_Work()
    {
        var myName = "Hadi";
        myName.Should().BeOfType<string>();

        var platform = new PlatformCreateDto(".NET", "Microsoft", "400$");

        platform.Should().NotBeNull();
    }
}
