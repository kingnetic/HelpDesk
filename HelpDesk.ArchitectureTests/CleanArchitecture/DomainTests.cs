using NetArchTest.Rules;
using Xunit;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.ArchitectureTests.Extensions;

namespace HelpDesk.ArchitectureTests.CleanArchitecture;

public class DomainTests
{
    [Fact]
    public void Domain_Should_Not_Reference_Application_Infrastructure_Or_API()
    {
        var result = Types
            .InAssembly(typeof(Ticket).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "HelpDesk.Application",
                "HelpDesk.Infrastructure",
                "HelpDesk.API")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_External_Libraries()
    {
        var forbidden = new[]
        {
            "Microsoft.EntityFrameworkCore",
            "MediatR",
            "FluentValidation",
            "Mapster",
            "Microsoft.AspNetCore.Identity"
        };

        var result = Types
            .InAssembly(typeof(Ticket).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetErrorMessages());
    }
}
