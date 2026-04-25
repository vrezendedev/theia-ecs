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
| Namespace                                  | Type                            | Relations | Mean       | Allocated |
|------------------------------------------- |-------------------------------- |---------- |-----------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloGetEvaluatedRelationOnT1  | 1         |   1.138 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaGetEvaluatedRelationOnT1   | 1         |   1.211 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchGetEvaluatedRelationOnT1    | 1         |   1.957 μs |      32 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsGetEvaluatedRelationOnT1 | 1         |   4.371 μs |     208 B |
|                                            |                                 |           |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaGetEvaluatedRelationOnT1   | 100       |  16.920 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchGetEvaluatedRelationOnT1    | 100       |  32.248 μs |    3200 B |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloGetEvaluatedRelationOnT1  | 100       | 120.908 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsGetEvaluatedRelationOnT1 | 100       | 468.283 μs |   20800 B |
