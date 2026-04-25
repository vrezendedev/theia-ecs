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
| Namespace                                     | Type                                  | EntityCount | Mean          | Gen0       | Gen1      | Gen2      | Allocated     |
|---------------------------------------------- |-------------------------------------- |------------ |--------------:|-----------:|----------:|----------:|--------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 16          |      14.46 μs |          - |         - |         - |       3.06 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 16          |      14.60 μs |          - |         - |         - |      16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 16          |      19.24 μs |          - |         - |         - |      10.41 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 16          |      20.92 μs |          - |         - |         - |       2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 16          |      61.89 μs |          - |         - |         - |      36.78 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 256         |      79.88 μs |          - |         - |         - |      16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 256         |     114.45 μs |          - |         - |         - |      36.34 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 256         |     232.08 μs |          - |         - |         - |      78.03 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 256         |     312.95 μs |          - |         - |         - |       2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 256         |     590.12 μs |          - |         - |         - |     204.77 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 512         |     238.59 μs |          - |         - |         - |      71.46 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 512         |     244.49 μs |          - |         - |         - |      22.58 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 512         |     333.42 μs |          - |         - |         - |     152.08 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 512         |     421.49 μs |          - |         - |         - |       2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 512         |   1,236.53 μs |          - |         - |         - |     674.86 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 1024        |     356.02 μs |          - |         - |         - |      50.77 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 1024        |     454.04 μs |          - |         - |         - |     141.58 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 1024        |     482.43 μs |          - |         - |         - |     313.96 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 1024        |     714.88 μs |          - |         - |         - |       2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 1024        |   2,107.43 μs |          - |         - |         - |    2399.09 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 4096        |     927.92 μs |          - |         - |         - |     171.45 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 4096        |   1,180.43 μs |          - |         - |         - |    1301.81 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 4096        |   1,291.26 μs |          - |         - |         - |     561.81 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 4096        |   2,741.36 μs |          - |         - |         - |       5.67 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 4096        |   7,529.23 μs |  2000.0000 |         - |         - |   34280.39 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 8192        |   1,144.64 μs |          - |         - |         - |    1121.93 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 8192        |   1,831.14 μs |          - |         - |         - |     332.16 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 8192        |   2,046.69 μs |          - |         - |         - |    2293.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 8192        |   5,124.90 μs |          - |         - |         - |       9.86 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 8192        |   9,394.49 μs |  8000.0000 |         - |         - |  134106.58 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 16384       |   3,292.11 μs |          - |         - |         - |      17.46 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 16384       |   3,744.74 μs |          - |         - |         - |     653.45 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 16384       |   3,744.91 μs |          - |         - |         - |    4620.27 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 16384       |   4,446.58 μs |          - |         - |         - |    2242.05 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 16384       |  24,229.19 μs | 32000.0000 |         - |         - |  530369.63 KB |
|                                               |                                       |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentDeferredOnT3      | 32768       |   2,053.19 μs |          - |         - |         - |    4482.16 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentDeferredOnT3     | 32768       |   2,600.45 μs |          - |         - |         - |    9298.45 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbRemoveComponentDeferredOnT3 | 32768       |   4,206.03 μs |          - |         - |         - |      34.21 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentDeferredOnT3      | 32768       |   7,216.93 μs |          - |         - |         - |    1295.87 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentDeferredOnT3       | 32768       | 214,399.25 μs | 57000.0000 | 4000.0000 | 3000.0000 | 2109251.99 KB |
