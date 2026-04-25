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
| Namespace                                     | Type                               | EntityCount | Mean          | Gen0       | Gen1      | Gen2      | Allocated     |
|---------------------------------------------- |----------------------------------- |------------ |--------------:|-----------:|----------:|----------:|--------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 16          |      17.79 μs |          - |         - |         - |       3.91 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 16          |      19.18 μs |          - |         - |         - |       17.9 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 16          |      20.41 μs |          - |         - |         - |       2.83 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 16          |      23.69 μs |          - |         - |         - |      14.55 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 16          |      51.52 μs |          - |         - |         - |      35.32 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 256         |     120.43 μs |          - |         - |         - |       17.9 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 256         |     132.17 μs |          - |         - |         - |      82.17 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 256         |     145.96 μs |          - |         - |         - |      44.88 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 256         |     315.14 μs |          - |         - |         - |       4.94 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 256         |     432.25 μs |          - |         - |         - |     174.43 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 512         |     269.76 μs |          - |         - |         - |      88.04 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 512         |     305.04 μs |          - |         - |         - |     156.22 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 512         |     335.41 μs |          - |         - |         - |      25.95 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 512         |     446.17 μs |          - |         - |         - |       7.19 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 512         |     769.88 μs |          - |         - |         - |     613.57 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 1024        |     450.77 μs |          - |         - |         - |      58.34 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 1024        |     472.87 μs |          - |         - |         - |      174.2 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 1024        |     564.89 μs |          - |         - |         - |     326.15 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 1024        |     849.98 μs |          - |         - |         - |      12.73 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 1024        |   1,467.99 μs |          - |         - |         - |    2275.91 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 4096        |     902.62 μs |          - |         - |         - |     690.53 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 4096        |   1,288.92 μs |          - |         - |         - |    1362.09 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 4096        |   1,445.09 μs |          - |         - |         - |     235.91 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 4096        |   3,105.54 μs |          - |         - |         - |      44.97 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 4096        |   6,301.97 μs |  2000.0000 |         - |         - |   33817.97 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 8192        |   2,175.38 μs |          - |         - |         - |    2418.26 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 8192        |   2,613.69 μs |          - |         - |         - |     1378.7 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 8192        |   2,685.35 μs |          - |         - |         - |     461.59 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 8192        |   6,083.70 μs |          - |         - |         - |      87.25 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 8192        |  10,301.14 μs |  8000.0000 |         - |         - |  133180.93 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 16384       |   3,121.59 μs |          - |         - |         - |     172.56 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 16384       |   3,953.93 μs |          - |         - |         - |    4872.64 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 16384       |   4,950.69 μs |          - |         - |         - |    2754.86 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 16384       |   5,327.00 μs |          - |         - |         - |     912.76 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 16384       |  23,555.07 μs | 32000.0000 |         - |         - |  528517.48 KB |
|                                               |                                    |             |               |            |           |           |               |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaAddComponentDeferredOnT3      | 32768       |   1,923.89 μs |          - |         - |         - |    1814.88 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloAddComponentDeferredOnT3     | 32768       |   2,536.27 μs |          - |         - |         - |    9806.88 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentAddComponentDeferredOnT3      | 32768       |   2,617.94 μs |          - |         - |         - |    5507.02 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbAddComponentDeferredOnT3 | 32768       |   6,068.38 μs |          - |         - |         - |     343.19 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchAddComponentDeferredOnT3       | 32768       | 207,949.76 μs | 56000.0000 | 3000.0000 | 3000.0000 | 2105553.51 KB |
