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
| Namespace                                     | Type                             | EntityCount | Mean         | Gen0      | Gen1      | Gen2      | Allocated   |
|---------------------------------------------- |--------------------------------- |------------ |-------------:|----------:|----------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 16          |     15.20 μs |         - |         - |         - |    16.15 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 16          |     30.95 μs |         - |         - |         - |     4.22 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 16          |     32.65 μs |         - |         - |         - |    18.73 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 16          |     59.04 μs |         - |         - |         - |    22.29 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 16          |     59.89 μs |         - |         - |         - |    11.42 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 256         |     73.61 μs |         - |         - |         - |    16.15 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 256         |    305.73 μs |         - |         - |         - |   138.62 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 256         |    336.25 μs |         - |         - |         - |    14.77 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 256         |    496.84 μs |         - |         - |         - |   107.92 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 256         |    585.68 μs |         - |         - |         - |   117.38 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 512         |    154.06 μs |         - |         - |         - |    40.17 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 512         |    512.11 μs |         - |         - |         - |    272.8 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 512         |    663.68 μs |         - |         - |         - |    26.02 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 512         |    850.53 μs |         - |         - |         - |   237.64 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 512         |  1,018.18 μs |         - |         - |         - |   204.25 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 1024        |    319.24 μs |         - |         - |         - |   104.58 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 1024        |    929.58 μs |         - |         - |         - |   570.92 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 1024        |  1,190.49 μs |         - |         - |         - |    49.56 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 1024        |  1,582.38 μs |         - |         - |         - |   477.88 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 1024        |  1,827.57 μs |         - |         - |         - |   428.86 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 4096        |    475.62 μs |         - |         - |         - |   490.41 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 4096        |  1,803.47 μs |         - |         - |         - |  2375.24 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 4096        |  4,376.34 μs |         - |         - |         - |  1727.06 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 4096        |  4,700.19 μs |         - |         - |         - |   302.89 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 4096        |  5,125.09 μs |         - |         - |         - |  1918.41 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 8192        |    673.18 μs |         - |         - |         - |   988.42 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 8192        |  2,423.94 μs |         - |         - |         - |   3838.7 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 8192        |  2,492.37 μs |         - |         - |         - |  4455.59 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 8192        |  7,169.13 μs |         - |         - |         - |  3457.53 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 8192        |  8,877.98 μs |         - |         - |         - |   602.22 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 16384       |  1,046.30 μs |         - |         - |         - |  1984.27 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 16384       |  3,367.43 μs |         - |         - |         - |  1201.63 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 16384       |  5,182.37 μs |         - |         - |         - |  7678.91 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 16384       |  6,313.38 μs | 2000.0000 | 2000.0000 | 2000.0000 |   8958.8 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 16384       |  7,063.33 μs |         - |         - |         - |  6919.65 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT5      | 32768       |  1,809.11 μs |         - |         - |         - |  4615.79 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT5 | 32768       |  6,970.65 μs |         - |         - |         - |  2400.44 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT5      | 32768       | 10,150.73 μs |         - |         - |         - | 15359.16 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT5     | 32768       | 11,291.47 μs | 2000.0000 | 2000.0000 | 2000.0000 | 17989.22 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT5       | 32768       | 13,668.35 μs |         - |         - |         - | 13846.58 KB |
