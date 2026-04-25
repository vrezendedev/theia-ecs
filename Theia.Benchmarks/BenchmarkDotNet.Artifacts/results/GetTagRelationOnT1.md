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
| Namespace                                  | Type                      | Relations | Mean        | Allocated |
|------------------------------------------- |-------------------------- |---------- |------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaGetTagRelationOnT1   | 1         |   0.9010 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloGetTagRelationOnT1  | 1         |   1.2031 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsGetTagRelationOnT1 | 1         |   1.4978 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchGetTagRelationOnT1    | 1         |   2.5537 μs |      32 B |
|                                            |                           |           |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaGetTagRelationOnT1   | 100       |  12.3381 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsGetTagRelationOnT1 | 100       |  26.0060 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchGetTagRelationOnT1    | 100       |  34.6200 μs |    3200 B |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloGetTagRelationOnT1  | 100       | 129.5065 μs |         - |
