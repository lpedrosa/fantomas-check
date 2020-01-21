module Sample.Program

open Sample

[<EntryPoint>]
let main argv =
    printfn "%s" (Greeting.say "Fantomas")
    0 // return an integer exit code
