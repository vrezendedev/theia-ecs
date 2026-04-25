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
| Namespace                                  | Type                            | Relations | Mean         | Allocated  |
|------------------------------------------- |-------------------------------- |---------- |-------------:|-----------:|
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaAddEvaluatedRelationOnT1   | 1         |     4.443 μs |    1.11 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloAddEvaluatedRelationOnT1  | 1         |    13.183 μs |   16.97 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsAddEvaluatedRelationOnT1 | 1         |    26.318 μs |    4.45 KB |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchAddEvaluatedRelationOnT1    | 1         |    40.772 μs |   51.44 KB |
|                                            |                                 |           |              |            |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaAddEvaluatedRelationOnT1   | 100       |    57.681 μs |    46.1 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloAddEvaluatedRelationOnT1  | 100       |   166.306 μs |   29.89 KB |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchAddEvaluatedRelationOnT1    | 100       |   189.799 μs |   81.07 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsAddEvaluatedRelationOnT1 | 100       | 5,575.894 μs | 4163.65 KB |
