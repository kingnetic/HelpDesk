using HelpDesk.Api.Controllers;
using HelpDesk.ArchitectureTests.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetArchTest.Rules;
using Xunit;

namespace HelpDesk.ArchitectureTests.CleanArchitecture;

public class APITests
{

    [Fact]
    public void API_Should_Not_Depened_On_Domain_Or_Infrastructure()
    {
        var forbidden = new[]
        {
            "HelpDesk.Domain",
            "HelpDesk.Infrastructure"
        };

        var result = Types
            .InAssembly(typeof(Program).Assembly)  // API assembly
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void API_Should_Depend_Only_On_Application_And_Allowed_Libraries()
    {
        var allowed = new[]
        {
        "HelpDesk.Application",
        "Microsoft.AspNetCore",
        "System",
        "Swashbuckle",
        "NetArchTest",
        "Newtonsoft",
        "Microsoft.Extensions"
    };

        var result = Types
            .InAssembly(typeof(Program).Assembly)
            .Should()
            .OnlyHaveDependenciesOn(allowed)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void AuthController_Should_Use_Only_Application_Service()
    {
        var result = Types
            .InAssembly(typeof(AuthController).Assembly)
            .That()
            .HaveName("AuthController")
            .Should()
            .HaveDependencyOn("HelpDesk.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void AuthController_Should_Not_Use_Forbidden_Infrastructure_Tech()
    {
        var forbidden = new[]
        {
            "Microsoft.AspNetCore.Identity",
            "Microsoft.IdentityModel",
            "System.IdentityModel",
            "Microsoft.Data.SqlClient",
            "HelpDesk.Infrastructure.Persistence"
        };

        var result = Types
            .InAssembly(typeof(AuthController).Assembly)
            .That()
            .HaveName("AuthController")
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }
}
