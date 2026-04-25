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
| Namespace                                     | Type                     | EntityCount | Mean          | Gen0      | Gen1      | Allocated   |
|---------------------------------------------- |------------------------- |------------ |--------------:|----------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 16          |      5.548 μs |         - |         - |     1.31 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 16          |     10.658 μs |         - |         - |     2.79 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 16          |     12.352 μs |         - |         - |    16.05 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 16          |     13.047 μs |         - |         - |      8.8 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 16          |     16.368 μs |         - |         - |    17.22 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 16          |    359.428 μs |         - |         - |   230.11 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 256         |     36.686 μs |         - |         - |    10.13 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 256         |     38.950 μs |         - |         - |    16.05 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 256         |     55.029 μs |         - |         - |     2.79 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 256         |     63.644 μs |         - |         - |    26.85 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 256         |     81.254 μs |         - |         - |    17.22 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 256         |  2,895.285 μs |         - |         - |   668.48 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 512         |     69.072 μs |         - |         - |    27.24 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 512         |     85.456 μs |         - |         - |    16.05 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 512         |    113.766 μs |         - |         - |     2.79 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 512         |    145.438 μs |         - |         - |    17.22 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 512         |    146.864 μs |         - |         - |    50.88 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 512         |  5,874.065 μs |         - |         - |  1135.88 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 1024        |    133.996 μs |         - |         - |    61.36 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 1024        |    157.238 μs |         - |         - |    32.27 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 1024        |    216.758 μs |         - |         - |     3.84 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 1024        |    295.232 μs |         - |         - |    49.42 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 1024        |    319.044 μs |         - |         - |   114.99 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 1024        |  2,841.125 μs |         - |         - |  2068.88 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 4096        |    243.706 μs |         - |         - |    97.09 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 4096        |    254.154 μs |         - |         - |   265.59 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 4096        |    808.392 μs |         - |         - |   120.07 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 4096        |    873.311 μs |         - |         - |   499.23 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 4096        |  1,082.346 μs |         - |         - |   178.38 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 4096        |  4,626.457 μs |         - |         - |  7673.88 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 8192        |    300.877 μs |         - |         - |   178.02 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 8192        |    438.373 μs |         - |         - |   537.71 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 8192        |  1,284.424 μs |         - |         - |  1011.34 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 8192        |  1,651.989 μs |         - |         - |    237.3 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 8192        |  2,137.177 μs |         - |         - |   339.78 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 8192        |  7,931.229 μs |         - |         - | 15689.03 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 16384       |    397.683 μs |         - |         - |   339.77 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 16384       |    634.856 μs |         - |         - |  1081.83 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 16384       |  2,547.233 μs |         - |         - |   472.52 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 16384       |  3,400.690 μs |         - |         - |  2035.46 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 16384       |  4,094.272 μs |         - |         - |   663.84 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 16384       | 15,449.414 μs | 1000.0000 |         - | 31241.53 KB |
|                                               |                          |             |               |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityT3      | 32768       |    796.131 μs |         - |         - |  1303.16 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityT3      | 32768       |    863.563 μs |         - |         - |  2169.95 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityT3       | 32768       |  1,289.300 μs |         - |         - |  1314.97 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityT3 | 32768       |  2,987.730 μs |         - |         - |   942.96 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityT3     | 32768       |  6,568.261 μs |         - |         - |  4083.58 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCreateEntityT3    | 32768       | 31,541.833 μs | 3000.0000 | 1000.0000 | 62665.95 KB |
