```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 5600 3.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

Method=Run  Job=Job-CNUJVU  InvocationCount=1  
IterationCount=Default  LaunchCount=Default  UnrollFactor=1  
WarmupCount=Default  

```
| Namespace                                     | Type                       | Mean     | Allocated |
|---------------------------------------------- |--------------------------- |---------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentWorldConstructor      | 10.00 μs |   8.88 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloWorldConstructor     | 13.79 μs | 390.97 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbWorldConstructor | 16.20 μs |  38.07 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchWorldConstructor       | 17.83 μs |  39.02 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaWorldConstructor      | 19.24 μs | 357.45 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsWorldConstructor    | 75.61 μs | 645.42 KB |
