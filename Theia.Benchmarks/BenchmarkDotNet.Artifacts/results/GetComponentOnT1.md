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
| Namespace                                     | Type                     | EntityCount | Mean          | Allocated |
|---------------------------------------------- |------------------------- |------------ |--------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 16          |     0.5679 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 16          |     1.1632 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 16          |     1.6064 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 16          |     1.7290 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 16          |     2.2839 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 16          |     7.9293 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 256         |     4.5100 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 256         |     9.7500 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 256         |    14.0429 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 256         |    15.7692 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 256         |    21.3385 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 256         |   106.0062 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 512         |     9.6188 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 512         |    19.6333 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 512         |    28.0821 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 512         |    34.5926 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 512         |    43.4447 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 512         |   198.3533 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 1024        |    19.6661 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 1024        |    39.0615 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 1024        |    56.8698 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 1024        |    70.5563 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 1024        |    88.3688 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 1024        |   383.9200 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 4096        |    32.0615 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 4096        |    70.6769 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 4096        |    83.7033 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 4096        |   118.5033 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 4096        |   155.1822 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 4096        |   231.8753 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 8192        |    48.4423 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 8192        |    89.0600 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 8192        |    91.9900 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 8192        |   102.0533 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 8192        |   143.2308 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 8192        |   366.7240 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 16384       |    81.3143 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 16384       |   130.3467 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 16384       |   143.5600 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 16384       |   150.7929 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 16384       |   204.6933 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 16384       |   729.7615 μs |         - |
|                                               |                          |             |               |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloGetComponentOnT1   | 32768       |   147.1143 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbGetComponentOnT1 | 32768       |   206.3385 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentGetComponentOnT1    | 32768       |   231.8667 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchGetComponentOnT1     | 32768       |   252.6012 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaGetComponentOnT1    | 32768       |   267.9933 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsGetComponentOnT1  | 32768       | 1,266.4786 μs |         - |
