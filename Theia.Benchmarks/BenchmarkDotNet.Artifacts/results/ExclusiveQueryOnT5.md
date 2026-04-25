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
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 16          |   1.246 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 16          |   2.796 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 16          |   3.172 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 16          |   3.492 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 16          |   4.833 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 16          |   9.893 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 256         |   4.447 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 256         |   8.935 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 256         |  11.547 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 256         |  11.667 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 256         |  21.850 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 256         |  22.317 μs |         - |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 512         |   7.334 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 512         |  16.275 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 512         |  18.411 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 512         |  19.494 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 512         |  58.335 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 512         |  71.583 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 1024        |  15.236 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 1024        |  31.993 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 1024        |  32.027 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 1024        |  36.230 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 1024        | 115.981 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 1024        | 167.540 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 4096        |  22.367 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 4096        |  38.531 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 4096        |  40.314 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 4096        |  43.329 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 4096        |  95.853 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 4096        | 192.282 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 8192        |  29.487 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 8192        |  49.480 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 8192        |  50.577 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 8192        |  52.770 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 8192        | 128.617 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 8192        | 128.926 μs |         - |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 16384       |  45.231 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 16384       |  70.864 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 16384       |  71.071 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 16384       |  71.557 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 16384       | 144.029 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 16384       | 201.340 μs |      72 B |
|                                               |                            |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT5    | 32768       |  76.454 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT5 | 32768       | 105.267 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT5   | 32768       | 112.813 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT5     | 32768       | 114.531 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT5    | 32768       | 122.366 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT5  | 32768       | 214.704 μs |      72 B |
