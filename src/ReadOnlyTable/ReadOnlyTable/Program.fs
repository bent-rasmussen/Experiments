open BenchmarkDotNet.Running
open Experiments

BenchmarkRunner.Run typeof<Benchmarks> |> ignore
