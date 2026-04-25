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
| Namespace                                  | Type                                  | Relations | Mean         | Allocated |
|------------------------------------------- |-------------------------------------- |---------- |-------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaRemoveEvaluatedRelationOnT1      | 1         |     1.392 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaBatchRemoveEvaluatedRelationOnT1 | 1         |     1.719 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloRemoveEvaluatedRelationOnT1     | 1         |     2.000 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsRemoveEvaluatedRelationOnT1    | 1         |     8.533 μs |     440 B |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchRemoveEvaluatedRelationOnT1       | 1         |     9.481 μs |     640 B |
|                                            |                                       |           |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaBatchRemoveEvaluatedRelationOnT1 | 100       |    10.692 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaRemoveEvaluatedRelationOnT1      | 100       |    26.246 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloRemoveEvaluatedRelationOnT1     | 100       |   109.129 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchRemoveEvaluatedRelationOnT1       | 100       |   143.617 μs |    6976 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsRemoveEvaluatedRelationOnT1    | 100       | 3,501.054 μs | 4084192 B |
