using NetArchTest.Rules;
using Xunit;
using HelpDesk.Application;
using HelpDesk.Domain.Entities.HelpDesk;

namespace HelpDesk.ArchitectureTests.CleanArchitecture;

public class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Have_Dependencies_On_Other_Layers()
    {
        var result = Types.InAssembly(typeof(Ticket).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "HelpDesk.Application",
                "HelpDesk.Infrastructure",
                "HelpDesk.API")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain layer has forbidden dependencies.");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_API()
    {
        var result = Types.InAssembly(typeof(DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "HelpDesk.Infrastructure",
                "HelpDesk.API")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application layer has forbidden dependencies.");
    }
}
