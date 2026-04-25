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
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 16          |     3.663 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 16          |     4.360 μs |     184 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 16          |     5.441 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 16          |     6.032 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 16          |     6.131 μs |     576 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 16          |    15.217 μs |     384 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 256         |    44.425 μs |    2200 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 256         |    45.667 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 256         |    66.738 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 256         |    69.722 μs |    2072 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 256         |    70.794 μs |    8352 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 256         |   188.993 μs |    6144 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 512         |    84.968 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 512         |   130.414 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 512         |   132.251 μs |   16568 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 512         |   182.375 μs |    4272 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 512         |   218.685 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 512         |   345.892 μs |   12288 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 1024        |   157.323 μs |      32 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 1024        |   168.543 μs |    8392 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 1024        |   257.050 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 1024        |   259.800 μs |    8216 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 1024        |   263.130 μs |   32976 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 1024        |   383.421 μs |   24576 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 4096        |   227.412 μs |   98304 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 4096        |   248.643 μs |     128 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 4096        |   373.131 μs |   32792 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 4096        |   442.287 μs |   33016 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 4096        |   731.600 μs |   49200 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 4096        |   862.656 μs |  131328 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 8192        |   316.585 μs |   33008 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 8192        |   435.629 μs |  196608 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 8192        |   516.521 μs |   65560 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 8192        |   791.185 μs |   65808 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 8192        | 1,397.058 μs |  114760 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 8192        | 1,608.200 μs |  262424 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 16384       |   485.608 μs |   98720 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 16384       |   805.427 μs |  131096 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 16384       |   843.367 μs |  524312 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 16384       | 1,268.803 μs |  131368 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 16384       | 2,666.008 μs |  245856 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 16384       | 3,208.693 μs |  524592 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT5    | 32768       |   748.509 μs | 1048904 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT5    | 32768       |   842.600 μs |  230096 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT5     | 32768       | 1,370.507 μs |  262168 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT5  | 32768       | 1,712.647 μs | 1179696 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT5   | 32768       | 2,414.164 μs |  262464 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT5 | 32768       | 5,213.115 μs |  508024 B |
