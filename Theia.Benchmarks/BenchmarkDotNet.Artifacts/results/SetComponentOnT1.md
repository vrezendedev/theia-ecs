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
| Namespace                                     | Type                     | EntityCount | Mean         | Allocated |
|---------------------------------------------- |------------------------- |------------ |-------------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 16          |     1.078 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 16          |     1.194 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 16          |     1.271 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 16          |     2.388 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 16          |     7.647 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 256         |     9.547 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 256         |     9.923 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 256         |    11.897 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 256         |    22.778 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 256         |   104.587 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 512         |    19.097 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 512         |    20.917 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 512         |    24.069 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 512         |    45.800 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 512         |   194.340 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 1024        |    38.294 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 1024        |    38.435 μs |         - |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 1024        |    41.150 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 1024        |    92.742 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 1024        |   382.762 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 4096        |    59.620 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 4096        |   109.750 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 4096        |   120.873 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 4096        |   149.288 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 4096        |   954.431 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 8192        |    75.971 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 8192        |    88.600 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 8192        |   134.357 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 8192        |   222.873 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 8192        |   372.809 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 16384       |   118.900 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 16384       |   146.829 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 16384       |   190.153 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 16384       |   431.927 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 16384       |   708.129 μs |         - |
|                                               |                          |             |              |           |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbSetComponentOnT1 | 32768       |   205.431 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaSetComponentOnT1    | 32768       |   261.210 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchSetComponentOnT1     | 32768       |   296.643 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloSetComponentOnT1   | 32768       |   827.367 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsSetComponentOnT1  | 32768       | 1,260.554 μs |         - |
