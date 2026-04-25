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
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 16          |     12.60 μs |         - |         - |         - |    15.97 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 16          |     14.72 μs |         - |         - |         - |     8.58 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 16          |     15.77 μs |         - |         - |         - |      3.8 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 16          |     18.35 μs |         - |         - |         - |     3.05 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 16          |     39.11 μs |         - |         - |         - |    35.09 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 256         |     54.12 μs |         - |         - |         - |    15.97 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 256         |    114.28 μs |         - |         - |         - |    98.09 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 256         |    158.68 μs |         - |         - |         - |     5.16 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 256         |    173.30 μs |         - |         - |         - |    49.36 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 256         |    290.11 μs |         - |         - |         - |    89.03 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 512         |    105.06 μs |         - |         - |         - |    31.99 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 512         |    288.06 μs |         - |         - |         - |   105.52 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 512         |    294.03 μs |         - |         - |         - |   200.18 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 512         |    313.25 μs |         - |         - |         - |     7.41 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 512         |    546.55 μs |         - |         - |         - |   152.17 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 1024        |    242.93 μs |         - |         - |         - |    64.02 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 1024        |    318.77 μs |         - |         - |         - |   217.69 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 1024        |    516.39 μs |         - |         - |         - |   418.11 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 1024        |    666.81 μs |         - |         - |         - |    11.91 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 1024        |    928.31 μs |         - |         - |         - |   278.31 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 4096        |    308.02 μs |         - |         - |         - |    304.2 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 4096        |    726.66 μs |         - |         - |         - |   890.02 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 4096        |  1,119.73 μs |         - |         - |         - |  1742.05 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 4096        |  2,470.18 μs |         - |         - |         - |  1131.16 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 4096        |  2,473.27 μs |         - |         - |         - |    154.1 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 8192        |    490.70 μs |         - |         - |         - |    608.3 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 8192        |  1,035.21 μs |         - |         - |         - |  1786.18 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 8192        |  1,446.70 μs |         - |         - |         - |  3182.22 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 8192        |  3,233.64 μs |         - |         - |         - |  2268.27 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 8192        |  4,241.93 μs |         - |         - |         - |   305.24 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 16384       |    803.49 μs |         - |         - |         - |   1216.5 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 16384       |  3,291.81 μs |         - |         - |         - |  3578.34 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 16384       |  4,584.35 μs | 1000.0000 | 1000.0000 | 1000.0000 |  6404.93 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 16384       |  4,928.20 μs |         - |         - |         - |  4542.93 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 16384       |  9,188.14 μs |         - |         - |         - |   607.52 KB |
|                                               |                                  |             |              |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT1      | 32768       |  1,440.71 μs |         - |         - |         - |  3072.82 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT1      | 32768       |  2,654.27 μs |         - |         - |         - |  7162.51 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT1       | 32768       |  7,930.82 μs |         - |         - |         - |  9095.92 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT1     | 32768       |  8,074.86 μs | 2000.0000 | 2000.0000 | 2000.0000 | 12879.11 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT1 | 32768       | 17,515.51 μs |         - |         - |         - |  1212.84 KB |
