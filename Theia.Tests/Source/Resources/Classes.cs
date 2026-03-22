namespace Theia.Tests.Resources;

public class NonBlittableClass() { }

public record NonBlittableRecord();

public class ManagedDataResource
{
    public int Value { get; set; }
}
