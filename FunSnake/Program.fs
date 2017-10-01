open FunSnake
open FunScript
open System
open System.IO
 
[<EntryPoint>]
let main argv = 
    let js = Compiler.compileWithoutReturn <@ main() @>
    File.WriteAllText(@"../../FunSnake.js",js) |> ignore
    Console.WriteLine("Javascript File generated")
    Console.ReadLine() |> ignore
    0

