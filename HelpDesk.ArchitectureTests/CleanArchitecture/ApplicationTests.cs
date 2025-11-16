using NetArchTest.Rules;
using Xunit;
using HelpDesk.ArchitectureTests.Extensions;
using HelpDesk.Application;

namespace HelpDesk.ArchitectureTests.CleanArchitecture;

public class ApplicationTests
{
    private static readonly string[] AllowedDeps =
    {
    "System",
    "Microsoft",
    "netstandard",
    "HelpDesk.Domain",
    "HelpDesk.Application",
    "MediatR",
    "FluentValidation",
    "Mapster"
    };

    [Fact]
    public void Application_Should_Only_Depend_On_Allowed_Layers()
    {
        var result = Types
            .InAssembly(typeof(DependencyInjection).Assembly)
            .Should()
            .OnlyHaveDependenciesOn(AllowedDeps)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_API()
    {
        var forbidden = new[]
        {
            "HelpDesk.Infrastructure",
            "HelpDesk.API",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore.Identity"
        };

        var result = Types
            .InAssembly(typeof(DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }
}
