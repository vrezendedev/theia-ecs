using System;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Theia.Benchmarks.Source.Columns;

[AttributeUsage(AttributeTargets.Method)]
public class WarningAttribute : Attribute
{
    public string Text { get; }

    public WarningAttribute(string text)
    {
        Text = text;
    }
}

public class WarningColumn : IColumn
{
    public string Id => nameof(WarningColumn);
    public string ColumnName => "Warning";

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public bool IsAvailable(Summary summary) => true;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        WarningAttribute? attr =
            benchmarkCase
                .Descriptor.WorkloadMethod.GetCustomAttributes(typeof(WarningAttribute), false)
                .FirstOrDefault() as WarningAttribute;

        return attr?.Text ?? "";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        WarningAttribute? attr =
            benchmarkCase
                .Descriptor.WorkloadMethod.GetCustomAttributes(typeof(WarningAttribute), false)
                .FirstOrDefault() as WarningAttribute;

        return attr?.Text ?? "";
    }

    public ColumnCategory Category => ColumnCategory.Custom;

    public int PriorityInCategory => int.MaxValue;

    public bool AlwaysShow => true;

    public string Legend => "Warning";

    public bool IsNumeric => false;

    public UnitType UnitType => UnitType.Dimensionless;
}
