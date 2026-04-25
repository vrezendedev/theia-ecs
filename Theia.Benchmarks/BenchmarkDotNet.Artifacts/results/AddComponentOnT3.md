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
| Namespace                                     | Type                     | EntityCount | Mean         | Gen0      | Allocated   |
|---------------------------------------------- |------------------------- |------------ |-------------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 16          |     12.06 μs |         - |     3.33 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 16          |     15.44 μs |         - |    16.76 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 16          |     15.60 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 16          |     18.29 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 16          |     40.24 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 16          |    115.19 μs |         - |    18.42 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 256         |    102.65 μs |         - |    16.76 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 256         |    109.26 μs |         - |     36.8 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 256         |    223.69 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 256         |    272.18 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 256         |    282.32 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 256         |  1,416.92 μs |         - |    225.3 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 512         |    204.19 μs |         - |    71.96 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 512         |    293.44 μs |         - |    16.76 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 512         |    299.46 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 512         |    470.27 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 512         |    532.63 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 512         |  2,760.45 μs |         - |    436.3 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 1024        |    377.67 μs |         - |    33.11 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 1024        |    388.89 μs |         - |   142.13 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 1024        |    488.08 μs |         - |    31.05 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 1024        |    655.82 μs |         - |     3.59 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 1024        |  1,062.81 μs |         - |    49.96 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 1024        |  2,118.95 μs |         - |    888.3 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 4096        |  1,128.48 μs |         - |   562.45 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 4096        |  1,248.39 μs |         - |   114.58 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 4096        |  1,821.75 μs |         - |   151.28 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 4096        |  1,885.94 μs |         - |   3540.3 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 4096        |  2,640.61 μs |         - |     8.83 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 4096        |  3,780.44 μs |         - |   211.85 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 8192        |  2,117.14 μs |         - |  1122.62 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 8192        |  2,464.83 μs |         - |   212.22 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 8192        |  3,575.94 μs |         - |   7076.3 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 8192        |  3,667.54 μs |         - |    311.4 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 8192        |  4,915.41 μs |         - |    15.11 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 8192        |  7,420.02 μs |         - |    390.7 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 16384       |    676.74 μs |         - |   631.52 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 16384       |  1,464.24 μs |         - |    28.42 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 16384       |  1,717.55 μs |         - |   751.13 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 16384       |  4,343.01 μs |         - |  2242.78 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 16384       |  4,887.92 μs |         - |   407.34 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 16384       |  7,184.73 μs |         - | 14148.24 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT3   | 32768       |  1,310.99 μs |         - |  1271.63 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT3    | 32768       |  1,740.34 μs |         - |  4482.95 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT3 | 32768       |  2,300.81 μs |         - |    55.05 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT3     | 32768       |  3,421.80 μs |         - |  1481.01 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT3    | 32768       |  9,479.55 μs |         - |   797.41 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT3  | 32768       | 14,228.01 μs | 1000.0000 | 28292.24 KB |
