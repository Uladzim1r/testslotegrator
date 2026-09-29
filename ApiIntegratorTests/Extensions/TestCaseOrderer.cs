using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace ApiIntegratorTests.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestOrderAttribute : Attribute
{
    public TestOrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}

public sealed class TestCaseOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var ordered = testCases
            .OrderBy(tc => GetOrder((IXunitTestCase)tc))
            .ThenBy(tc => ((IXunitTestCase)tc).TestMethod.Method.Name)
            .ToList();

        return ordered;
    }

    private static int GetOrder(IXunitTestCase testCase)
    {
        return testCase.TestMethod.Method.GetCustomAttributes<TestOrderAttribute>().FirstOrDefault()?.Order ?? int.MaxValue;
    }
}
