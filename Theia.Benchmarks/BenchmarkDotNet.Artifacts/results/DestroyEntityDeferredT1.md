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
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 16          |     3.871 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 16          |     5.069 μs |     776 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 16          |     5.682 μs |     664 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 16          |     6.359 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 16          |    14.091 μs |     520 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 256         |    31.617 μs |    6616 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 256         |    33.483 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 256         |    41.542 μs |   11432 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 256         |    60.586 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 256         |   111.233 μs |   39208 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 512         |    76.268 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 512         |    78.757 μs |   22720 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 512         |   170.971 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 512         |   172.668 μs |   86408 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 512         |   177.808 μs |   12808 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 1024        |   154.189 μs |   25144 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 1024        |   168.740 μs |   45272 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 1024        |   192.344 μs |   12336 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 1024        |   263.265 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 1024        |   309.742 μs |  180712 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 4096        |   201.018 μs |   61608 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 4096        |   358.379 μs |   98968 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 4096        |   438.269 μs |  180488 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 4096        |   657.906 μs |  746152 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 4096        |   732.213 μs |   49200 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 8192        |   290.600 μs |  160016 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 8192        |   606.350 μs |  197320 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 8192        |   803.774 μs |  360736 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 8192        |   998.351 μs | 1499912 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 8192        | 1,325.214 μs |  114760 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 16384       |   465.981 μs |  356760 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 16384       | 1,178.014 μs |  393976 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 16384       | 1,476.020 μs |  721208 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 16384       | 1,973.727 μs | 3007336 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 16384       | 2,701.277 μs |  245856 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT1    | 32768       |   777.747 μs |  750176 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT1   | 32768       |   848.602 μs |  787240 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT1    | 32768       |   853.057 μs | 1442128 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT1     | 32768       | 3,630.810 μs | 6022088 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT1 | 32768       | 5,290.362 μs |  508024 B |
