﻿// This test checks that EasyPost C# code can be used in F#.
// This test project is running on .NET 6.0, although a success here should mean a success in all versions of .NET.'

namespace EasyPost.Tests.FSharp

open System
open EasyPost
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type FSharpCompileTest () =
    [<TestMethod>]
    member this.TestCompile() =
        // The assert doesn't really do anything, but as long as this test can run, then the code is compiling correctly.
        let result = Assert.ThrowsException<ClientNotConfigured>(fun() -> Console.Write(CarrierType.All()); new obj())
        Assert.IsNotNull(result)