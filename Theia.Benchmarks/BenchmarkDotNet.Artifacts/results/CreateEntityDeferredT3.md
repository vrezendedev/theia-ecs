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
| Namespace                                     | Type                             | EntityCount | Mean        | Gen0      | Gen1      | Gen2      | Allocated   |
|---------------------------------------------- |--------------------------------- |------------ |------------:|----------:|----------:|----------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 16          |    15.98 μs |         - |         - |         - |    16.05 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 16          |    22.73 μs |         - |         - |         - |     3.63 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 16          |    28.15 μs |         - |         - |         - |    13.66 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 16          |    33.91 μs |         - |         - |         - |     7.03 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 16          |    44.37 μs |         - |         - |         - |    20.77 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 256         |    60.55 μs |         - |         - |         - |    16.05 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 256         |   229.11 μs |         - |         - |         - |   118.35 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 256         |   288.32 μs |         - |         - |         - |     9.96 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 256         |   385.45 μs |         - |         - |         - |    79.05 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 256         |   395.87 μs |         - |         - |         - |    91.96 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 512         |   138.94 μs |         - |         - |         - |    36.08 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 512         |   401.71 μs |         - |         - |         - |   236.49 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 512         |   455.36 μs |         - |         - |         - |    16.71 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 512         |   539.63 μs |         - |         - |         - |   163.24 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 512         |   744.32 μs |         - |         - |         - |    173.2 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 1024        |   234.89 μs |         - |         - |         - |    92.31 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 1024        |   732.60 μs |         - |         - |         - |   494.52 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 1024        |   834.96 μs |         - |         - |         - |   331.48 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 1024        |   926.06 μs |         - |         - |         - |    31.26 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 1024        | 1,445.64 μs |         - |         - |         - |   367.63 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 4096        |   366.15 μs |         - |         - |         - |   397.19 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 4096        | 1,594.55 μs |         - |         - |         - |  2058.65 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 4096        | 2,890.12 μs |         - |         - |         - |   1339.9 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 4096        | 3,306.65 μs |         - |         - |         - |   228.49 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 4096        | 3,568.11 μs |         - |         - |         - |  1469.05 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 8192        |   553.39 μs |         - |         - |         - |   798.14 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 8192        | 2,162.76 μs |         - |         - |         - |  3818.91 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 8192        | 4,980.61 μs |         - |         - |         - |   2926.7 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 8192        | 5,569.57 μs |         - |         - |         - |  2684.11 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 8192        | 6,914.59 μs |         - |         - |         - |   453.73 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 16384       |   909.21 μs |         - |         - |         - |  1599.91 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 16384       | 2,955.76 μs |         - |         - |         - |   904.95 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 16384       | 3,047.18 μs |         - |         - |         - |  5372.32 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 16384       | 5,141.99 μs |         - |         - |         - |  5842.83 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 16384       | 5,380.13 μs | 1000.0000 | 1000.0000 | 1000.0000 |  7681.71 KB |
|                                               |                                  |             |             |           |           |           |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCreateEntityDeferredT3      | 32768       | 1,603.12 μs |         - |         - |         - |  3843.32 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntitiesDbCreateEntityDeferredT3 | 32768       | 4,525.41 μs |         - |         - |         - |  1807.38 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCreateEntityDeferredT3     | 32768       | 4,736.44 μs | 2000.0000 | 2000.0000 | 2000.0000 | 15432.69 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCreateEntityDeferredT3      | 32768       | 6,490.60 μs |         - |         - |         - | 10748.51 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCreateEntityDeferredT3       | 32768       | 9,896.56 μs |         - |         - |         - | 11678.16 KB |
