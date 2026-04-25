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
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 16          |   1.238 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 16          |   2.909 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 16          |   2.975 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 16          |   3.216 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 16          |   5.502 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 16          |  10.036 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 256         |   3.606 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 256         |   8.355 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 256         |   9.821 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 256         |  10.241 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 256         |  15.150 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 256         |  19.707 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 512         |   5.913 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 512         |  15.444 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 512         |  17.291 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 512         |  18.400 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 512         |  31.518 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 512         |  43.946 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 1024        |  11.599 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 1024        |  29.431 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 1024        |  31.040 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 1024        |  35.061 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 1024        |  62.893 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 1024        |  91.936 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 4096        |  16.436 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 4096        |  32.546 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 4096        |  33.793 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 4096        |  36.659 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 4096        |  66.131 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 4096        | 163.026 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 8192        |  20.358 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 8192        |  37.571 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 8192        |  38.653 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 8192        |  38.964 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 8192        |  74.190 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 8192        | 250.027 μs |      72 B |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 16384       |  29.838 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 16384       |  44.482 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 16384       |  47.907 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 16384       |  52.931 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 16384       |  61.845 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 16384       |  88.573 μs |         - |
|                                               |                                 |             |            |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaCrossArchetypeQueryOnT3    | 32768       |  49.371 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbCrossArchetypeQueryOnT3 | 32768       |  57.843 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloCrossArchetypeQueryOnT3   | 32768       |  67.527 μs |      56 B |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchCrossArchetypeQueryOnT3     | 32768       |  76.326 μs |     704 B |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsCrossArchetypeQueryOnT3  | 32768       |  94.484 μs |      72 B |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentCrossArchetypeQueryOnT3    | 32768       | 125.400 μs |         - |
