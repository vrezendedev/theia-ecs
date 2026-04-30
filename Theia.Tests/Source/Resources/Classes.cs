namespace Theia.Tests.Resources;

public class NonBlittableClass() { }

public record NonBlittableRecord();

public sealed class TestResource
{
    public int Value { get; init; }
}
