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
| Namespace                                     | Type                            | EntityCount | Mean       | Allocated |
|---------------------------------------------- |-------------------------------- |------------ |-----------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 16          |   1.514 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 16          |   3.466 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 16          |   3.615 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 16          |   3.913 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 16          |   5.322 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 16          |  13.557 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 256         |   3.617 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 256         |  10.317 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 256         |  12.799 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 256         |  17.444 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 256         |  25.243 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 256         |  25.891 μs |         - |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 512         |   7.793 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 512         |  17.150 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 512         |  19.561 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 512         |  20.164 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 512         |  51.118 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 512         |  75.184 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 1024        |  15.740 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 1024        |  32.375 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 1024        |  34.524 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 1024        |  37.321 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 1024        | 105.675 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 1024        | 119.384 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 4096        |  20.639 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 4096        |  39.508 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 4096        |  41.083 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 4096        |  44.296 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 4096        | 103.788 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 4096        | 332.586 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 8192        |  30.621 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 8192        |  51.857 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 8192        |  52.086 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 8192        |  56.050 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 8192        |  68.182 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 8192        | 116.172 μs |         - |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 16384       |  48.064 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 16384       |  70.714 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 16384       |  71.907 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 16384       |  72.743 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 16384       |  86.519 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 16384       | 137.321 μs |         - |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT5    | 32768       |  83.150 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT5 | 32768       | 107.535 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT5     | 32768       | 114.037 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT5   | 32768       | 114.820 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT5    | 32768       | 129.436 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT5  | 32768       | 159.184 μs |      72 B |
