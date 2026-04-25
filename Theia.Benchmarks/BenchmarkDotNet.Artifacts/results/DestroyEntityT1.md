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
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 16          |     2.743 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 16          |     3.351 μs |     184 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 16          |     3.924 μs |     576 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 16          |     4.504 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 16          |     4.936 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 16          |     7.647 μs |     384 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 256         |    23.514 μs |    2200 B |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 256         |    26.852 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 256         |    36.965 μs |    8352 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 256         |    37.493 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 256         |    57.382 μs |    2072 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 256         |    75.826 μs |    6144 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 512         |    55.748 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 512         |    71.323 μs |   16568 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 512         |   100.650 μs |    4120 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 512         |   127.789 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 512         |   143.094 μs |    4272 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 512         |   152.984 μs |   12288 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 1024        |   116.200 μs |    8392 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 1024        |   138.065 μs |   32976 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 1024        |   168.164 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 1024        |   195.290 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 1024        |   198.195 μs |    8216 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 1024        |   306.976 μs |   24576 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 4096        |   205.703 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 4096        |   264.717 μs |   32792 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 4096        |   308.709 μs |   33016 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 4096        |   372.163 μs |  131328 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 4096        |   446.100 μs |   49200 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 4096        |   629.892 μs |   98304 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 8192        |   190.518 μs |   32920 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 8192        |   239.621 μs |  196608 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 8192        |   353.320 μs |   65560 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 8192        |   553.743 μs |   65808 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 8192        |   707.160 μs |  262424 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 8192        |   773.673 μs |  114760 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 16384       |   271.000 μs |   98568 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 16384       |   450.650 μs |  524312 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 16384       |   534.069 μs |  131096 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 16384       | 1,033.280 μs |  131368 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 16384       | 1,351.796 μs |  524592 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 16384       | 1,447.620 μs |  245856 B |
|                                               |                         |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaDestroyEntityT1    | 32768       |   455.087 μs |  229816 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchDestroyEntityT1     | 32768       |   864.683 μs |  262168 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsDestroyEntityT1  | 32768       |   902.624 μs | 1179696 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloDestroyEntityT1   | 32768       | 2,078.193 μs |  262464 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentDestroyEntityT1    | 32768       | 2,565.129 μs | 1048904 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbDestroyEntityT1 | 32768       | 2,835.153 μs |  508024 B |
