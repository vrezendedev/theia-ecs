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

| Namespace                                     | Type                       | EntityCount |        Mean | Allocated |
| --------------------------------------------- | -------------------------- | ----------- | ----------: | --------: |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 16          |   0.8612 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 16          |   1.8784 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 16          |   2.1448 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 16          |   2.7479 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 16          |   4.5062 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 16          |   5.6958 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 256         |   2.5970 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 256         |   7.0429 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 256         |   7.3476 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 256         |   9.1571 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 256         |   9.6830 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 256         |  12.3385 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 512         |   4.4990 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 512         |  12.8034 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 512         |  13.3278 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 512         |  14.8560 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 512         |  17.6689 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 512         |  23.0191 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 1024        |   8.5237 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 1024        |  24.4706 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 1024        |  25.2364 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 1024        |  26.5370 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 1024        |  33.2764 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 1024        |  40.4867 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 4096        |  10.2977 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 4096        |  27.8357 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 4096        |  28.0061 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 4096        |  28.5538 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 4096        |  40.4533 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 4096        |  47.1962 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 8192        |  12.6041 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 8192        |  29.6929 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 8192        |  31.7929 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 8192        |  32.8214 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 8192        |  45.1167 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 8192        |  55.8980 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 16384       |  17.7600 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 16384       |  31.3174 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 16384       |  37.3317 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 16384       |  42.2929 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 16384       |  57.5857 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 16384       |  77.5647 μs |      72 B |
|                                               |                            |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaExclusiveQueryOnT1    | 32768       |  23.7206 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbExclusiveQueryOnT1 | 32768       |  38.1429 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchExclusiveQueryOnT1     | 32768       |  52.1414 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloExclusiveQueryOnT1   | 32768       |  60.8786 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentExclusiveQueryOnT1    | 32768       |  80.8643 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsExclusiveQueryOnT1  | 32768       | 112.2538 μs |      72 B |
