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
| Namespace                                 | Type                               | Relations | Mean      | Allocated |
|------------------------------------------ |----------------------------------- |---------- |----------:|----------:|
| Theia.Benchmarks.Source.Frameworks.Friflo | FrifloGetAllEvaluatedRelationsOnT1 | 1         | 0.7370 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch   | ArchGetAllEvaluatedRelationsOnT1   | 1         | 0.9360 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia  | TheiaGetAllEvaluatedRelationsOnT1  | 1         | 1.0303 μs |         - |
|                                           |                                    |           |           |           |
| Theia.Benchmarks.Source.Frameworks.Friflo | FrifloGetAllEvaluatedRelationsOnT1 | 100       | 0.8220 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Arch   | ArchGetAllEvaluatedRelationsOnT1   | 100       | 1.0808 μs |         - |
| Theia.Benchmarks.Source.Frameworks.Theia  | TheiaGetAllEvaluatedRelationsOnT1  | 100       | 1.0920 μs |         - |
