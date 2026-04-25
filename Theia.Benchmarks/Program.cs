using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

// using BenchmarkDotNet.Columns;
// using Theia.Benchmarks.Source.Columns;

ManualConfig config = DefaultConfig
    .Instance.WithOption(ConfigOptions.JoinSummary, true)
    .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest))
    .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)
    .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond))
    .AddDiagnoser(MemoryDiagnoser.Default)
    // .AddColumn(new WarningColumn())
    .HideColumns(
        "Method",
        "Error",
        "StdDev",
        "Median",
        "RatioSD",
        "InvocationCount",
        "IterationCount",
        "UnrollFactor",
        "Job",
        "LaunchCount",
        "WarmupCount",
        "Alloc Ratio"
    )
    .AddExporter(new CategoryNamedExporter(CsvExporter.Default))
    .AddExporter(new CategoryNamedExporter(HtmlExporter.Default))
    .AddExporter(new CategoryNamedExporter(MarkdownExporter.GitHub));

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
