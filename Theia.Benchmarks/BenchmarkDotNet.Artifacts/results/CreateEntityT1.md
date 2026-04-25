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
| Namespace                                     | Type                     | EntityCount | Mean          | Allocated  |
|---------------------------------------------- |------------------------- |------------ |--------------:|-----------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 16          |      3.855 μs |      784 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 16          |     10.221 μs |     2840 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 16          |     10.296 μs |    16352 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 16          |     12.302 μs |     4776 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 16          |     21.939 μs |    34040 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 16          |     69.608 μs |   207264 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 256         |     21.200 μs |     5776 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 256         |     41.545 μs |    16352 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 256         |     56.130 μs |     2840 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 256         |     61.217 μs |    23256 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 256         |     74.401 μs |    34040 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 256         |    765.920 μs |   329016 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 512         |     43.433 μs |    19160 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 512         |     73.338 μs |    16352 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 512         |     94.946 μs |     2840 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 512         |    128.567 μs |    34040 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 512         |    144.101 μs |    47856 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 512         |  1,566.784 μs |   459064 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 1024        |     83.236 μs |    45856 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 1024        |    135.049 μs |    16352 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 1024        |    198.060 μs |     2840 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 1024        |    238.554 μs |    34040 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 1024        |    313.115 μs |   105272 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 1024        |  3,056.529 μs |   719160 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 4096        |    176.419 μs |   205744 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 4096        |    178.808 μs |    65648 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 4096        |    757.641 μs |   120792 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 4096        |    883.785 μs |   449480 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 4096        |    910.069 μs |   132920 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 4096        | 10,497.893 μs |  2279736 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 8192        |    248.179 μs |   114880 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 8192        |    293.907 μs |   418808 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 8192        |  1,276.662 μs |   908304 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 8192        |  1,418.379 μs |   238696 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 8192        |  2,002.008 μs |   264984 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 8192        |  3,266.086 μs |  4753744 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 16384       |    351.884 μs |   213360 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 16384       |    557.654 μs |   844864 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 16384       |  1,615.028 μs |  1825880 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 16384       |  2,593.463 μs |   474504 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 16384       |  3,787.514 μs |   529880 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 16384       |  4,061.431 μs |  9898600 B |
|                                               |                          |             |               |            |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT1      | 32768       |    705.293 μs |  1065632 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT1      | 32768       |  1,099.630 μs |  1696904 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT1 | 32768       |  5,201.308 μs |   946888 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT1     | 32768       |  6,473.771 μs |  3660960 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT1       | 32768       |  7,071.580 μs |  1063256 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT1    | 32768       |  8,010.575 μs | 19597696 B |
