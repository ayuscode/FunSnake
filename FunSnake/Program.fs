open FunSnake
open FunScript
open System
open System.IO
 
[<EntryPoint>]
let main argv = 
    let js = Compiler.compileWithoutReturn <@ main() @>
    File.WriteAllText(@"C:\Users\alber_000\Documents\Visual Studio 2013\Projects\_GitHub\FunSnake\FunSnake\FunSnake.js",js) |> ignore
    Console.WriteLine("Javascript File generated")
    Console.ReadLine() |> ignore
    0

