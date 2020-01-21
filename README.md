# fantomas-check task example

This is an example project using [Fantomas](https://github.com/fsprojects/fantomas) `FakeHelpers` module to build a FAKE task which can be used for integrating in a CI pipeline.

The idea is for the FAKE task to fail if there are any files that need formatting.

## Building the project

```
dotnet tool restore
dotnet fake build
```

## Running the task

The build script exposes a `CheckFormat` task. This task fails if any source file requires formatting, while also outputting their file names:

```
dotnet fake build -t CheckFormat
```

Sample output:

```
run CheckFormat
Building project with version: LocalBuild
Shortened DependencyGraph for Target CheckFormat:
<== CheckFormat

The running order is:
Group - 1
  - CheckFormat
Starting target 'CheckFormat'
The following files need formatting:
/app/src/Sample/Greeting.fs
Finished (Failed) 'CheckFormat' in 00:00:00.5686893

---------------------------------------------------------------------
Build Time Report
---------------------------------------------------------------------
Target        Duration
------        --------
CheckFormat   00:00:00.5648007   (Some files need formatting, check output for more info)
Total:        00:00:00.6272949
Status:       Failure
---------------------------------------------------------------------
Script reported an error:
-> BuildFailedException: Target 'CheckFormat' failed.
-> One or more errors occurred. (Some files need formatting, check output for more info)
-> Some files need formatting, check output for more info
Hint: To further diagnose the problem you can run fake in verbose mode `fake -v run ...` or set the 'FAKE_DETAILED_ERRORS' environment variable to 'true'
Performance:
 - Cli parsing: 215 milliseconds
 - Packages: 44 milliseconds
 - Script analyzing: 13 milliseconds
 - Script running: 751 milliseconds
 - Script cleanup: 0 milliseconds
 - Runtime: 1 second
```

## License

MIT