namespace Experiments

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open System.ComponentModel

// Inspired by https://www.youtube.com/watch?v=QW71imIn28s

[<Struct>]
type ReadOnlyTable0<'TKey, 'TValue>(store: Dictionary<'TKey, 'TValue>) =

    member this.Item
        with get k =
            store[k]

[<Struct>]
type ReadOnlyTable1<'TKey, 'TValue>(store: Dictionary<'TKey, 'TValue>) =

    /// Implementation property exposing mutable state.
    /// Never use this.
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    member this._internal = store

    member inline this.Item
        with get k =
            this._internal[k]

[<Struct>]
type ReadOnlyTable2<'TKey, 'TValue>(store: Dictionary<'TKey, 'TValue>) =

    member this.Item
        with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get k =
            store[k]

[<SimpleJob(RuntimeMoniker.Net70)>]
type Benchmarks() =

    let indices = new List<int>()
    let data = new Dictionary<int, int>()

    [<Params(100, 1_000, 10_000, 100_000, 1_000_000)>]
    member val size = 0 with get, set

    [<GlobalSetup>]
    member this.GlobalSetup() =
        for i = 0 to this.size do
            let index = Random.Shared.Next(0, this.size)
            let value = i
            indices.Add(index)
            data[index] <- value

    [<Benchmark(Baseline = true)>]
    member this.Dictionary() =
        let dict = data
        for i = 0 to this.size do
            let index = indices[Random.Shared.Next(0, this.size)]
            let x = dict[index]
            ()

    member this.ReadOnlyDictionary() =
        let dict = data.AsReadOnly()
        for i = 0 to this.size do
            let index = indices[Random.Shared.Next(0, this.size)]
            let x = dict[index]
            ()

    [<Benchmark()>]
    member this.ReadOnlyTable0() =
        let rot = new ReadOnlyTable0<_, _>(data)
        for i = 0 to this.size do
            let index = indices[Random.Shared.Next(0, this.size)]
            let x = rot[index]
            ()

    [<Benchmark()>]
    member this.ReadOnlyTable1() =
        let rot = new ReadOnlyTable1<_, _>(data)
        for i = 0 to this.size do
            let index = indices[Random.Shared.Next(0, this.size)]
            let x = rot[index]
            ()

    [<Benchmark()>]
    member this.ReadOnlyTable2() =
        let rot = new ReadOnlyTable2<_, _>(data)
        for i = 0 to this.size do
            let index = indices[Random.Shared.Next(0, this.size)]
            let x = rot[index]
            ()
