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
| Namespace                                     | Type                            | EntityCount | Mean        | Allocated |
|---------------------------------------------- |-------------------------------- |------------ |------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 16          |   0.9714 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 16          |   1.9145 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 16          |   2.5539 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 16          |   3.0948 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 16          |   5.2990 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 16          |   7.1908 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 256         |   2.8440 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 256         |   7.2676 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 256         |   8.0265 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 256         |   9.2750 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 256         |  10.4062 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 256         |  13.4714 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 512         |   4.7240 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 512         |  13.0804 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 512         |  13.9645 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 512         |  15.4394 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 512         |  17.2345 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 512         |  23.8723 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 1024        |   9.4778 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 1024        |  24.6396 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 1024        |  26.5934 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 1024        |  27.6500 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 1024        |  34.6540 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 1024        |  43.0450 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 4096        |  10.5897 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 4096        |  26.9333 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 4096        |  28.2067 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 4096        |  28.4923 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 4096        |  40.3462 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 4096        |  81.5794 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 8192        |  14.1125 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 8192        |  30.1143 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 8192        |  31.9000 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 8192        |  33.2143 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 8192        |  45.6071 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 8192        | 102.3312 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 16384       |  16.6472 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 16384       |  31.4238 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 16384       |  39.3367 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 16384       |  43.0267 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 16384       |  57.8846 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 16384       | 118.3636 μs |      72 B |
|                                               |                                 |             |             |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT1    | 32768       |  25.4108 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT1 | 32768       |  37.8250 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT1     | 32768       |  55.6112 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT1   | 32768       |  61.9267 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT1    | 32768       |  83.5400 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT1  | 32768       | 133.9500 μs |      72 B |
