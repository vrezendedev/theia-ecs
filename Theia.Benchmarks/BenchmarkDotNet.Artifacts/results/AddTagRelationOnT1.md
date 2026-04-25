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
| Namespace                                  | Type                      | Relations | Mean         | Allocated  |
|------------------------------------------- |-------------------------- |---------- |-------------:|-----------:|
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaAddTagRelationOnT1   | 1         |     4.393 μs |    1.07 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloAddTagRelationOnT1  | 1         |    13.200 μs |   12.96 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsAddTagRelationOnT1 | 1         |    26.344 μs |    4.35 KB |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchAddTagRelationOnT1    | 1         |    40.394 μs |   51.43 KB |
|                                            |                           |           |              |            |
| Theia.Benchmarks.Source.Frameworks.Theia   | TheiaAddTagRelationOnT1   | 100       |    54.508 μs |   44.91 KB |
| Theia.Benchmarks.Source.Frameworks.Arch    | ArchAddTagRelationOnT1    | 100       |   190.591 μs |   80.34 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo  | FrifloAddTagRelationOnT1  | 100       |   265.011 μs |   25.88 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs | FennecsAddTagRelationOnT1 | 100       | 4,037.213 μs | 3690.21 KB |
