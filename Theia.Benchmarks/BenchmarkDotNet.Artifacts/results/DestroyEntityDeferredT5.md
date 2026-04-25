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
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 16          |     4.702 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 16          |     6.478 μs |     664 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 16          |     7.225 μs |     776 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 16          |     7.916 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 16          |    14.830 μs |     520 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 256         |    46.494 μs |    6616 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 256         |    49.957 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 256         |    70.400 μs |   11432 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 256         |    84.100 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 256         |   106.983 μs |   39208 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 512         |   138.161 μs |   22720 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 512         |   160.178 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 512         |   172.712 μs |   86408 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 512         |   202.917 μs |   12808 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 512         |   248.524 μs |         - |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 1024        |   191.381 μs |   12368 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 1024        |   197.481 μs |   25144 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 1024        |   285.935 μs |   45272 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 1024        |   310.081 μs |  180712 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 1024        |   312.314 μs |         - |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 4096        |   301.187 μs |   61664 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 4096        |   471.435 μs |   98968 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 4096        |   694.029 μs |  746152 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 4096        |   866.987 μs |  180488 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 4096        | 1,093.187 μs |   49200 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 8192        |   412.415 μs |  160104 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 8192        |   759.807 μs |  197320 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 8192        | 1,293.250 μs | 1499912 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 8192        | 1,628.083 μs |  360736 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 8192        | 2,122.586 μs |  114760 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 16384       |   698.015 μs |  356912 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 16384       | 1,400.180 μs |  393976 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 16384       | 2,328.085 μs | 3007336 B |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 16384       | 3,122.115 μs |  721208 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 16384       | 3,846.121 μs |  245856 B |
|                                             |                                 |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Frent    | FrentDestroyEntityDeferredT5    | 32768       | 1,050.707 μs | 1442128 B |
| Theia.Benchmarks.Source.Frameworks.Theia    | TheiaDestroyEntityDeferredT5    | 32768       | 1,204.023 μs |  750456 B |
| Theia.Benchmarks.Source.Frameworks.EntityDb | EntityDbDestroyEntityDeferredT5 | 32768       | 2,078.136 μs |  508024 B |
| Theia.Benchmarks.Source.Frameworks.Friflo   | FrifloDestroyEntityDeferredT5   | 32768       | 2,460.979 μs |  787240 B |
| Theia.Benchmarks.Source.Frameworks.Arch     | ArchDestroyEntityDeferredT5     | 32768       | 3,199.395 μs | 6021896 B |
