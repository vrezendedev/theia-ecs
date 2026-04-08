using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
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
    );

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
