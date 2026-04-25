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
| Namespace                                   | Type                            | EntityCount | Mean         | Allocated |
|-------------------------------------------- |-------------------------------- |------------ |-------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 16          |     4.477 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 16          |     5.897 μs |     664 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 16          |     6.152 μs |     776 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 16          |     7.321 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 16          |    13.750 μs |     520 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 256         |    46.127 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 256         |    50.886 μs |    6616 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 256         |    58.599 μs |   11432 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 256         |    70.760 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 256         |   113.455 μs |   39208 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 512         |    86.144 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 512         |   106.762 μs |   22720 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 512         |   174.846 μs |   86408 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 512         |   187.403 μs |   12808 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 512         |   217.925 μs |         - |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 1024        |   164.973 μs |   25144 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 1024        |   224.179 μs |   45272 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 1024        |   235.116 μs |   12368 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 1024        |   288.000 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 1024        |   318.604 μs |  180712 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 4096        |   239.873 μs |   61664 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 4096        |   431.840 μs |   98968 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 4096        |   660.192 μs |  180488 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 4096        |   739.423 μs |  746152 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 4096        |   885.840 μs |   49200 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 8192        |   328.762 μs |  160104 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 8192        |   666.447 μs |  197320 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 8192        | 1,148.957 μs |  360736 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 8192        | 1,210.050 μs | 1499912 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 8192        | 1,655.492 μs |  114760 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 16384       |   555.308 μs |  356912 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 16384       | 1,158.314 μs |  393976 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 16384       | 2,159.531 μs | 3007336 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 16384       | 2,225.840 μs |  721208 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 16384       | 3,628.443 μs |  245856 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT3    | 32768       |   913.158 μs |  750456 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT3 | 32768       | 1,672.657 μs |  508024 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT3   | 32768       | 2,089.464 μs |  787240 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT3     | 32768       | 3,341.400 μs | 6021896 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT3    | 32768       | 4,982.160 μs | 1442128 B |
