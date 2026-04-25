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
| Namespace                                  | Type                            | Relations | Mean         | Allocated |
|------------------------------------------- |-------------------------------- |---------- |-------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaRemoveTagRelationOnT1      | 1         |     1.462 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaBatchRemoveTagRelationOnT1 | 1         |     1.733 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloRemoveTagRelationOnT1     | 1         |     2.066 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsRemoveTagRelationOnT1    | 1         |     8.193 μs |     440 B |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchRemoveTagRelationOnT1       | 1         |    10.303 μs |     640 B |
|                                            |                                 |           |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaBatchRemoveTagRelationOnT1 | 100       |     9.358 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaRemoveTagRelationOnT1      | 100       |    27.162 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloRemoveTagRelationOnT1     | 100       |   114.593 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchRemoveTagRelationOnT1       | 100       |   143.564 μs |    6976 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsRemoveTagRelationOnT1    | 100       | 3,359.175 μs | 3608992 B |
