using NetArchTest.Rules;
using Xunit;
using HelpDesk.Infrastructure;
using HelpDesk.ArchitectureTests.Extensions;

namespace HelpDesk.ArchitectureTests.CleanArchitecture;

public class InfrastructureTests
{
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_API()
    {
        var result = Types
            .InAssembly(typeof(DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn("HelpDesk.API")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void Infrastructure_Should_Not_Use_Application_Handlers_Or_Validators()
    {
        var forbidden = new[]
        {
        "HelpDesk.Application.Handlers",
        "HelpDesk.Application.Validators"
    };

        var result = Types
            .InAssembly(typeof(DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }


}
