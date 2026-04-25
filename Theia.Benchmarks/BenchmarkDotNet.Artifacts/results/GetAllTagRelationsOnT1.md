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
| Namespace                                 | Type                         | Relations | Mean      | Allocated |
|------------------------------------------ |----------------------------- |---------- |----------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Friflo | FrifloGetAllTagRelationsOnT1 | 1         | 0.7300 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia  | TheiaGetAllTagRelationsOnT1  | 1         | 0.7660 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch   | ArchGetAllTagRelationsOnT1   | 1         | 1.0809 μs |         - |
|                                           |                              |           |           |           |
| Theia.Benchmarks.Source.Frameworks.Friflo | FrifloGetAllTagRelationsOnT1 | 100       | 0.7240 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia  | TheiaGetAllTagRelationsOnT1  | 100       | 0.8505 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch   | ArchGetAllTagRelationsOnT1   | 100       | 1.1439 μs |         - |
