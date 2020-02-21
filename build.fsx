#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fantomas 3.3.0-beta-002 //"
#load ".fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fantomas
open Fantomas.FormatConfig

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

let fantomasConfig = FormatConfig.Default

Target.create "CheckCodeFormat" (fun _ ->
    let toErrorline (filename, exn) =
        sprintf "%s\n %s" filename (exn.ToString())

    let needFormatting =
        !! "src/**/*.fs"
        -- "src/**/obj/**"
        |> FakeHelpers.checkCode fantomasConfig
        |> Async.RunSynchronously

    if needFormatting.IsValid then
        Trace.log "No files need formatting"
    else
        if needFormatting.HasErrors then
            Trace.log "The following files had errors while formatting:"
            needFormatting.Errors |> List.iter (toErrorline >> Trace.log)
            failwith "Some files had errors while formatting, check output for more info"
        else
            Trace.log "The following files need formatting:"
            needFormatting.Formatted |> List.iter Trace.log
            failwith "Some files need formatting, check output for more info"
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
