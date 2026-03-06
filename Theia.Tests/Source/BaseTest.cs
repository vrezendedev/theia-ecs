using Xunit.Abstractions;

namespace Theia.Tests;

public abstract class BaseTest
{
    protected readonly ITestOutputHelper Output;

    public BaseTest(ITestOutputHelper testOutputHelper) => Output = testOutputHelper;
}
