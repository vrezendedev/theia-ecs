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
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 16          |     14.73 μs |         - |     3.92 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 16          |     18.17 μs |         - |    16.95 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 16          |     19.12 μs |         - |     2.57 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 16          |     20.55 μs |         - |    15.07 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 16          |     38.64 μs |         - |    17.77 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 16          |    153.62 μs |         - |    22.35 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 256         |    128.14 μs |         - |    16.95 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 256         |    145.86 μs |         - |    41.33 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 256         |    258.32 μs |         - |    15.07 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 256         |    304.20 μs |         - |     2.57 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 256         |    358.33 μs |         - |    17.77 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 256         |  1,810.58 μs |         - |   270.48 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 512         |    256.28 μs |         - |    80.54 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 512         |    310.83 μs |         - |    15.07 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 512         |    353.75 μs |         - |    33.44 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 512         |    402.20 μs |         - |     2.57 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 512         |    515.93 μs |         - |   521.48 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 512         |    696.62 μs |         - |    50.63 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 1024        |    465.84 μs |         - |   158.75 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 1024        |    475.48 μs |         - |    50.11 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 1024        |    590.60 μs |         - |    43.23 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 1024        |    779.58 μs |         - |     3.62 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 1024        |  1,356.74 μs |         - |    66.38 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 1024        |  1,545.15 μs |         - |  1065.48 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 4096        |  1,534.09 μs |         - |   627.17 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 4096        |  1,592.51 μs |         - |   149.17 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 4096        |  2,211.12 μs |         - |   211.56 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 4096        |  2,302.64 μs |         - |  4149.48 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 4096        |  3,023.87 μs |         - |      9.9 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 4096        |  4,957.83 μs |         - |   228.75 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 8192        |  3,082.26 μs |         - |  1251.38 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 8192        |  3,172.06 μs |         - |   280.96 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 8192        |  4,272.49 μs |         - |   435.73 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 8192        |  4,379.68 μs |         - |  8485.42 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 8192        |  6,040.68 μs |         - |    18.27 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 8192        | 11,041.28 μs |         - |    441.5 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 16384       |    768.71 μs |         - |   883.89 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 16384       |  1,250.00 μs |         - |    35.77 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 16384       |  2,363.89 μs |         - |    868.7 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 16384       |  5,718.85 μs |         - |  2499.59 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 16384       |  5,965.01 μs |         - |   544.33 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 16384       |  8,847.14 μs |         - | 16581.42 KB |
|                                               |                          |             |              |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT5   | 32768       |  1,522.85 μs |         - |  1780.05 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT5    | 32768       |  1,994.63 μs |         - |  1070.85 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT5    | 32768       |  2,022.48 μs |         - |   4995.8 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT5 | 32768       |  2,595.75 μs |         - |    70.77 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT5     | 32768       |  4,674.57 μs |         - |  1732.11 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT5  | 32768       | 17,900.83 μs | 1000.0000 | 33029.42 KB |
