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
| Namespace                                     | Type                       | EntityCount | Mean       | Allocated |
|---------------------------------------------- |--------------------------- |------------ |-----------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 16          |   1.065 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 16          |   2.499 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 16          |   2.685 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 16          |   3.005 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 16          |   4.693 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 16          |   8.606 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 256         |   3.381 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 256         |   8.841 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 256         |  10.809 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 256         |  12.786 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 256         |  15.050 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 256         |  17.023 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 512         |   5.589 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 512         |  14.973 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 512         |  16.694 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 512         |  16.883 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 512         |  32.222 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 512         |  45.310 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 1024        |  28.522 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 1024        |  29.074 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 1024        |  31.442 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 1024        |  48.178 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 1024        |  61.681 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 1024        |  91.323 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 4096        |  14.677 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 4096        |  31.831 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 4096        |  32.908 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 4096        |  33.462 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 4096        |  62.382 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 4096        |  72.471 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 8192        |  17.217 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 8192        |  36.900 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 8192        |  37.327 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 8192        |  39.121 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 8192        |  80.980 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 8192        | 151.833 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 16384       |  26.125 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 16384       |  43.933 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 16384       |  47.779 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 16384       |  49.556 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 16384       |  87.007 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 16384       | 115.392 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT3    | 32768       |  41.214 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT3 | 32768       |  56.473 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT3   | 32768       |  68.193 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT3     | 32768       |  75.214 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT3    | 32768       | 119.827 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT3  | 32768       | 232.021 μs |      72 B |
