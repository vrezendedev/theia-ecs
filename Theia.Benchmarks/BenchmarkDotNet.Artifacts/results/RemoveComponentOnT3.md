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
| Namespace                                     | Type                        | EntityCount | Mean         | Gen0      | Allocated   |
|---------------------------------------------- |---------------------------- |------------ |-------------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 16          |     10.51 μs |         - |     2.73 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 16          |     14.19 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 16          |     14.45 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 16          |     15.96 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 16          |     37.95 μs |         - |    33.66 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 16          |     85.59 μs |         - |    11.41 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 256         |     69.60 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 256         |     92.94 μs |         - |    32.27 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 256         |    179.49 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 256         |    258.66 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 256         |    264.79 μs |         - |    33.66 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 256         |  1,046.96 μs |         - |   142.54 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 512         |    179.45 μs |         - |    63.38 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 512         |    204.88 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 512         |    254.55 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 512         |    364.77 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 512         |    482.45 μs |         - |    33.66 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 512         |  2,018.82 μs |         - |   274.54 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 1024        |    249.91 μs |         - |    32.72 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 1024        |    342.79 μs |         - |    125.5 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 1024        |    414.78 μs |         - |    18.86 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 1024        |    608.82 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 1024        |    928.81 μs |         - |     49.8 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 1024        |    954.43 μs |         - |   546.54 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 4096        |    864.53 μs |         - |    81.35 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 4096        |  1,178.97 μs |         - |   497.73 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 4096        |  1,370.15 μs |         - |  2178.54 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 4096        |  1,536.34 μs |         - |       91 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 4096        |  2,253.67 μs |         - |     5.67 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 4096        |  3,548.37 μs |         - |   179.03 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 8192        |  1,099.51 μs |         - |     9.86 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 8192        |  1,700.57 μs |         - |   146.04 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 8192        |  2,193.84 μs |         - |   993.85 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 8192        |  2,334.23 μs |         - |  4354.54 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 8192        |  3,082.15 μs |         - |   187.07 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 8192        |  6,910.12 μs |         - |   325.15 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 16384       |  1,495.30 μs |         - |   620.13 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 16384       |  3,527.07 μs |         - |    275.3 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 16384       |  4,306.93 μs |         - |  1985.97 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 16384       |  4,520.54 μs |         - |  8706.48 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 16384       |  5,958.35 μs |         - |   379.14 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 16384       |  9,063.31 μs |         - |    17.46 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT3   | 32768       |  1,135.22 μs |         - |   763.21 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT3    | 32768       |  1,526.86 μs |         - |  3970.09 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT3 | 32768       |  2,024.31 μs |         - |    34.21 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT3     | 32768       |  2,923.95 μs |         - |   1219.1 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT3    | 32768       |  6,667.40 μs |         - |    533.7 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT3  | 32768       | 10,110.10 μs | 1000.0000 | 17922.48 KB |
