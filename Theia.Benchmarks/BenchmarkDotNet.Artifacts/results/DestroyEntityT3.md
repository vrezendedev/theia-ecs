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
| Namespace                                     | Type                    | EntityCount | Mean         | Allocated |
|---------------------------------------------- |------------------------ |------------ |-------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 16          |     3.290 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 16          |     3.984 μs |     184 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 16          |     5.061 μs |     576 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 16          |     5.323 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 16          |     5.339 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 16          |    11.220 μs |     384 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 256         |    29.600 μs |    2200 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 256         |    32.392 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 256         |    53.163 μs |    8352 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 256         |    59.679 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 256         |    61.842 μs |    2072 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 256         |   139.326 μs |    6144 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 512         |    73.793 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 512         |   101.145 μs |   16568 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 512         |   119.800 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 512         |   167.229 μs |    4272 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 512         |   178.344 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 512         |   234.658 μs |   12288 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 1024        |   141.286 μs |    8392 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 1024        |   144.056 μs |      32 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 1024        |   200.941 μs |   32976 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 1024        |   225.122 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 1024        |   228.179 μs |    8216 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 1024        |   359.731 μs |   24576 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 4096        |   179.593 μs |     128 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 4096        |   311.756 μs |   32792 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 4096        |   350.100 μs |   98304 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 4096        |   368.714 μs |   33016 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 4096        |   609.395 μs |  131328 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 4096        |   612.743 μs |   49200 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 8192        |   233.871 μs |   33008 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 8192        |   333.200 μs |  196608 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 8192        |   444.821 μs |   65560 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 8192        |   614.138 μs |   65808 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 8192        | 1,157.607 μs |  262424 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 8192        | 1,162.271 μs |  114760 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 16384       |   358.825 μs |   98720 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 16384       |   657.995 μs |  524312 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 16384       |   679.300 μs |  131096 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 16384       | 1,168.367 μs |  131368 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 16384       | 2,141.393 μs |  245856 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 16384       | 2,166.208 μs |  524592 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT3    | 32768       |   594.847 μs |  230096 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT3    | 32768       |   654.470 μs | 1048904 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT3     | 32768       | 1,119.354 μs |  262168 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT3  | 32768       | 1,307.604 μs | 1179696 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT3   | 32768       | 2,243.092 μs |  262464 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT3 | 32768       | 4,111.250 μs |  508024 B |
