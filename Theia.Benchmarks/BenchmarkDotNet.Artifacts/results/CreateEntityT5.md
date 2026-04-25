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
| Namespace                                     | Type                     | EntityCount | Mean          | Gen0      | Gen1      | Allocated    |
|---------------------------------------------- |------------------------- |------------ |--------------:|----------:|----------:|-------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 16          |      7.616 μs |         - |         - |      1.86 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 16          |     10.996 μs |         - |         - |     16.15 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 16          |     11.700 μs |         - |         - |      2.81 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 16          |     16.467 μs |         - |         - |     12.95 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 16          |     17.726 μs |         - |         - |      17.3 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 16          |    585.800 μs |         - |         - |    266.29 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 256         |     46.581 μs |         - |         - |     16.15 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 256         |     50.640 μs |         - |         - |     14.61 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 256         |     62.994 μs |         - |         - |      2.81 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 256         |     66.338 μs |         - |         - |     30.99 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 256         |     97.573 μs |         - |         - |      17.3 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 256         |  6,078.260 μs |         - |         - |   1117.31 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 512         |     92.777 μs |         - |         - |     16.15 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 512         |     93.402 μs |         - |         - |     35.77 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 512         |    126.354 μs |         - |         - |      2.81 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 512         |    158.835 μs |         - |         - |     55.02 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 512         |    171.131 μs |         - |         - |      17.3 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 512         | 11,492.583 μs |         - |         - |   2024.06 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 1024        |    181.723 μs |         - |         - |     32.53 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 1024        |    192.705 μs |         - |         - |     77.94 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 1024        |    281.165 μs |         - |         - |      3.86 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 1024        |    327.980 μs |         - |         - |    127.18 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 1024        |    363.207 μs |         - |         - |     49.59 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 1024        |  2,686.700 μs |         - |         - |   3837.44 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 4096        |    241.887 μs |         - |         - |    130.31 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 4096        |    321.866 μs |         - |         - |    330.27 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 4096        |    822.502 μs |         - |         - |    559.51 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 4096        |    998.619 μs |         - |         - |    122.19 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 4096        |  1,376.454 μs |         - |         - |    195.13 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 4096        |  7,965.136 μs |         - |         - |  14722.06 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 8192        |    352.653 μs |         - |         - |     244.3 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 8192        |    469.684 μs |         - |         - |    666.43 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 8192        |  1,229.479 μs |         - |         - |   1135.67 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 8192        |  1,768.690 μs |         - |         - |    241.52 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 8192        |  2,621.085 μs |         - |         - |    389.41 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 8192        | 15,084.433 μs | 1000.0000 |         - |  29617.71 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 16384       |    483.176 μs |         - |         - |    472.13 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 16384       |    711.546 μs |         - |         - |   1338.59 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 16384       |  1,506.835 μs |         - |         - |   2287.84 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 16384       |  2,712.232 μs |         - |         - |    480.92 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 16384       |  5,119.580 μs |         - |         - |    779.23 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 16384       | 30,948.371 μs | 3000.0000 | 1000.0000 |  59409.84 KB |
|                                               |                          |             |               |           |           |              |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT5      | 32768       |    905.658 μs |         - |         - |   1567.63 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT5       | 32768       |  1,559.887 μs |         - |         - |   1561.85 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT5      | 32768       |  1,979.129 μs |         - |         - |   2682.76 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT5     | 32768       |  5,963.914 μs |         - |         - |      4592 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT5 | 32768       |  7,820.800 μs |         - |         - |    959.73 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT5    | 32768       | 59,751.647 μs | 7000.0000 | 1000.0000 | 118993.86 KB |
