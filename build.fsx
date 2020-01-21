#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fantomas 3.1.0 //"
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

let removeTemporary (results: FakeHelpers.FormatResult []): unit = 
    let removeIfHasTemporary result = 
        match result with
        | FakeHelpers.Formatted(_, tempFile) -> File.Delete(tempFile)
        | FakeHelpers.Error(_) | FakeHelpers.Unchanged(_) -> ()
    results |> Array.iter removeIfHasTemporary 

let checkCodeAndReport (config: FormatConfig) (files: seq<string>): Async<string[]> =
    async {
        let! results = files |> FakeHelpers.formatFilesAsync config
        results |> removeTemporary

        let toChange result = 
            match result with
            | FakeHelpers.Formatted(file, _) -> Some(file, None)
            | FakeHelpers.Error(file, ex) -> Some(file, Some(ex))
            | FakeHelpers.Unchanged(_) -> None

        let changes =
            results
            |> Array.choose toChange
        
        let isChangeWithErrors = function
        | _, Some(_) -> true
        | _, None -> false

        if Array.exists isChangeWithErrors changes then
            raise <| FakeHelpers.CodeFormatException changes

        let formattedFilename = function 
        | _, Some(_) -> None
        | filename, None -> Some(filename)

        return 
            changes
            |> Array.choose formattedFilename
    }

Target.create "CheckCodeFormat" (fun _ ->
    let needFormatting = 
        !! "src/**/*.fs"
        -- "src/**/obj/**"
        |> checkCodeAndReport fantomasConfig
        |> Async.RunSynchronously
    
    match Array.length needFormatting with
    | 0 -> Trace.log "No files need formatting"
    | _ -> 
        Trace.log "The following files need formatting:"
        needFormatting |> Array.iter Trace.log 
        failwith "Some files need formatting, check output for more info"
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
