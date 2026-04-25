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
| Namespace                                     | Type                     | EntityCount | Mean          | Gen0      | Allocated   |
|---------------------------------------------- |------------------------- |------------ |--------------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 16          |      9.500 μs |         - |     2.73 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 16          |     12.729 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 16          |     13.776 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 16          |     15.964 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 16          |     27.728 μs |         - |    17.59 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 16          |    135.450 μs |         - |    13.13 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 256         |     75.144 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 256         |     82.295 μs |         - |    32.27 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 256         |    148.208 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 256         |    176.211 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 256         |    189.640 μs |         - |    17.59 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 256         |    983.963 μs |         - |   166.63 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 512         |    160.146 μs |         - |    63.38 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 512         |    238.166 μs |         - |    16.55 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 512         |    252.657 μs |         - |     6.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 512         |    375.092 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 512         |    375.700 μs |         - |    17.59 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 512         |  1,946.925 μs |         - |   330.63 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 1024        |    302.916 μs |         - |    125.5 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 1024        |    339.115 μs |         - |    32.69 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 1024        |    403.815 μs |         - |    18.86 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 1024        |    542.030 μs |         - |     2.53 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 1024        |    738.067 μs |         - |    50.32 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 1024        |  3,761.169 μs |         - |   670.63 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 4096        |    805.674 μs |         - |   497.73 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 4096        |    917.771 μs |         - |     81.3 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 4096        |  1,408.205 μs |         - |  2626.63 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 4096        |  1,491.033 μs |         - |       91 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 4096        |  1,947.243 μs |         - |     5.67 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 4096        |  2,531.179 μs |         - |   162.83 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 8192        |  1,533.464 μs |         - |   993.85 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 8192        |  1,803.857 μs |         - |   145.95 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 8192        |  2,448.896 μs |         - |   308.82 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 8192        |  2,921.546 μs |         - |   187.07 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 8192        |  3,052.792 μs |         - |  5346.63 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 8192        |  4,133.069 μs |         - |     9.86 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 16384       |  2,767.262 μs |         - |  1985.97 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 16384       |  3,302.469 μs |         - |   604.08 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 16384       |  3,550.686 μs |         - |   275.15 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 16384       |  5,042.836 μs |         - | 10498.63 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 16384       |  5,751.857 μs |         - |   379.14 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 16384       |  7,768.721 μs |         - |    18.23 KB |
|                                               |                          |             |               |           |             |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentOnT1   | 32768       |  1,191.852 μs |         - |   763.21 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentOnT1    | 32768       |  1,402.497 μs |         - |  3970.09 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbAddComponentOnT1 | 32768       |  1,794.807 μs |         - |    35.73 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentOnT1     | 32768       |  2,072.820 μs |         - |  1203.05 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentOnT1    | 32768       |  6,916.413 μs |         - |   533.42 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsAddComponentOnT1  | 32768       | 10,618.167 μs | 1000.0000 | 21378.63 KB |
