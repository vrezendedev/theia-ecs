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
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 16          |     12.80 μs |         - |     3.33 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 16          |     15.30 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 16          |     16.65 μs |         - |    16.74 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 16          |     19.02 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 16          |     44.07 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 16          |    127.88 μs |         - |    16.28 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 256         |    102.16 μs |         - |    16.74 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 256         |    165.60 μs |         - |     36.8 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 256         |    210.20 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 256         |    229.15 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 256         |    321.23 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 256         |  1,468.60 μs |         - |   198.03 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 512         |    236.21 μs |         - |    71.96 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 512         |    263.57 μs |         - |    16.74 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 512         |    339.47 μs |         - |    10.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 512         |    370.30 μs |         - |     2.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 512         |    612.90 μs |         - |    33.76 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 512         |    812.06 μs |         - |   380.03 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 1024        |    343.59 μs |         - |    33.09 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 1024        |    445.80 μs |         - |   142.13 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 1024        |    508.53 μs |         - |    31.05 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 1024        |    520.68 μs |         - |   756.03 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 1024        |    707.65 μs |         - |     3.59 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 1024        |  1,217.96 μs |         - |    49.96 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 4096        |  1,280.08 μs |         - |   114.56 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 4096        |  1,622.35 μs |         - |   562.45 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 4096        |  1,936.34 μs |         - |  3011.98 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 4096        |  1,950.68 μs |         - |   151.28 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 4096        |  2,664.13 μs |         - |     8.83 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 4096        |  4,721.46 μs |         - |   195.65 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 8192        |  2,518.10 μs |         - |    212.2 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 8192        |  3,132.84 μs |         - |  1122.62 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 8192        |  3,417.58 μs |         - |  6211.98 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 8192        |  3,678.74 μs |         - |    311.4 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 8192        |  5,175.63 μs |         - |    15.11 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 8192        |  9,293.70 μs |         - |   374.37 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 16384       |    688.50 μs |         - |   631.52 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 16384       |  2,141.95 μs |         - |   734.55 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 16384       |  2,792.44 μs |         - |    28.42 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 16384       |  4,730.20 μs |         - |   407.32 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 16384       |  6,025.40 μs |         - |  2242.78 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 16384       |  6,743.04 μs |         - | 12419.98 KB |
|                                               |                             |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT5   | 32768       |  1,415.59 μs |         - |  1271.63 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT5    | 32768       |  1,865.31 μs |         - |  4482.95 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT5     | 32768       |  4,267.59 μs |         - |  1463.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT5 | 32768       |  5,899.43 μs |         - |    55.05 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT5    | 32768       |  9,921.54 μs |         - |   797.39 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT5  | 32768       | 14,182.78 μs | 1000.0000 | 24835.98 KB |
