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
| Namespace                                     | Type                        | EntityCount | Mean         | P95           | Allocated   |
|---------------------------------------------- |---------------------------- |------------ |-------------:|--------------:|------------:|
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 16          |     8.687 μs |      9.150 μs |     2.44 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 16          |    10.979 μs |     17.810 μs |    16.45 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 16          |    13.555 μs |     18.060 μs |     4.72 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 16          |    15.202 μs |     17.600 μs |     2.52 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 16          |    34.974 μs |     46.715 μs |    33.62 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 16          |    74.112 μs |     79.010 μs |     9.38 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 256         |    49.308 μs |     50.340 μs |    16.45 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 256         |    80.421 μs |     93.450 μs |       30 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 256         |    91.269 μs |     93.525 μs |     4.72 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 256         |   214.643 μs |    218.035 μs |    33.62 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 256         |   228.586 μs |    304.315 μs |     2.52 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 256         |   835.860 μs |    844.650 μs |   118.13 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 512         |   158.068 μs |    269.240 μs |    16.45 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 512         |   161.390 μs |    182.500 μs |    59.09 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 512         |   192.570 μs |    196.070 μs |     4.72 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 512         |   354.221 μs |    431.625 μs |     2.52 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 512         |   408.481 μs |    421.100 μs |    33.62 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 512         | 1,622.983 μs |  1,658.680 μs |   228.13 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 1024        |   195.577 μs |    208.915 μs |    16.48 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 1024        |   288.399 μs |    311.840 μs |   117.19 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 1024        |   376.861 μs |    392.390 μs |    12.77 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 1024        |   545.443 μs |    560.500 μs |     2.52 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 1024        |   783.771 μs |  1,481.730 μs |   454.13 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 1024        |   800.652 μs |    820.600 μs |    33.11 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 4096        |   714.321 μs |    735.690 μs |    64.71 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 4096        |   946.908 μs |    986.205 μs |   465.38 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 4096        | 1,137.173 μs |  1,482.085 μs |  1810.13 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 4096        | 1,379.400 μs |  1,408.960 μs |    60.86 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 4096        | 2,136.446 μs |  2,172.920 μs |     5.66 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 4096        | 2,920.264 μs |  2,962.545 μs |   130.43 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 8192        |   896.539 μs |  1,970.145 μs |   929.47 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 8192        | 1,354.277 μs |  1,370.040 μs |   112.88 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 8192        | 1,835.513 μs |  1,964.925 μs |  3618.13 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 8192        | 2,683.515 μs |  2,713.880 μs |   124.91 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 8192        | 4,126.807 μs |  4,142.330 μs |      8.8 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 8192        | 5,975.120 μs |  6,103.640 μs |   260.15 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 16384       | 2,698.275 μs |  2,806.270 μs |    209.2 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 16384       | 3,246.807 μs | 11,911.760 μs |   521.31 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 16384       | 3,684.000 μs |  3,710.715 μs |  7426.13 KB |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 16384       | 3,696.760 μs |  3,747.700 μs |  1857.56 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 16384       | 5,609.314 μs |  5,647.335 μs |   252.95 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 16384       | 8,093.993 μs |  8,149.370 μs |    15.09 KB |
|                                               |                             |             |              |               |             |
| Theia.Benchmarks.Source.Frameworks.Frent      | FrentRemoveComponentOnT2    | 32768       | 1,346.312 μs |  1,395.175 μs |  3713.66 KB |
| Theia.Benchmarks.Source.Frameworks.EntitiesDb | EntityDbRemoveComponentOnT2 | 32768       | 1,888.881 μs |  2,016.610 μs |     28.4 KB |
| Theia.Benchmarks.Source.Frameworks.Arch       | ArchRemoveComponentOnT2     | 32768       | 2,261.751 μs |  2,321.010 μs |  1053.69 KB |
| Theia.Benchmarks.Source.Frameworks.Friflo     | FrifloRemoveComponentOnT2   | 32768       | 3,309.827 μs |  3,350.180 μs |      509 KB |
| Theia.Benchmarks.Source.Frameworks.Theia      | TheiaRemoveComponentOnT2    | 32768       | 5,417.793 μs |  5,517.390 μs |   401.74 KB |
| Theia.Benchmarks.Source.Frameworks.Fennecs    | FennecsRemoveComponentOnT2  | 32768       | 7,140.407 μs |  7,239.100 μs | 14850.07 KB |
