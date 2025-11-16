using NetArchTest.Rules;

namespace HelpDesk.ArchitectureTests.Extensions;

public static class TestResultExtensions
{
    public static string GetErrorMessages(this TestResult result)
    {
        if (result == null)
            return "TestResult is null.";

        if (result.IsSuccessful)
            return "OK";

        if (result.FailingTypes == null || !result.FailingTypes.Any())
            return "Unknown architecture violation.";

        return string.Join(Environment.NewLine,
            result.FailingTypes.Select(t => $" - {t.FullName}"));
    }

    public static string ErrorsToString(this TestResult result)
    {
        if (result == null || result.FailingTypes == null || !result.FailingTypes.Any())
            return "Unknown architecture violation.";

        return string.Join(Environment.NewLine,
            result.FailingTypes.Select(f => " - " + f.FullName));
    }

    private static string Errors(TestResult result)
    {
        if (result == null || result.FailingTypes == null || !result.FailingTypes.Any())
            return "Unknown architecture violation.";

        return string.Join(Environment.NewLine,
            result.FailingTypes.Select(x => " - " + x.FullName));
    }
}
