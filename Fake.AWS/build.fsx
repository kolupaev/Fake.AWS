#r "packages/FAKE/tools/FakeLib.dll" // include Fake lib
open Fake
open Fake.Testing

let buildDir = "./build"

Target "Clean" (fun _ -> CleanDirs [buildDir; ])

Target "BuildSolution" (fun _ ->
    MSBuildRelease buildDir "Build" ["./Fake.AWS.sln"]
    |> Log "AppBuild-Output: "
)

Target "Merge" (fun _ ->
    let mergeOutDir = buildDir @@ "merged"
    FileHelper.CreateDir mergeOutDir 
    ILMergeHelper.ILMerge
      (fun p -> { p with Libraries = (!!(buildDir @@ "AWSSDK.*.dll")); Internalize = InternalizeTypes.Internalize }) 
      (mergeOutDir @@ "Fake.AWS.dll")
      (buildDir @@ "Fake.AWS.dll")
)

Target "Test" (fun _ ->
    let test = (buildDir @@ "test")
    FileHelper.CreateDir test
    !! (buildDir @@ @"*.fsx")
      ++ (buildDir @@ "merged" @@ "Fake.AWS.dll") 
      |> FileHelper.CopyFiles test
    !! (buildDir + @"\*.Tests.dll")
      |> xUnit (fun s -> {s with ToolPath = @"packages\xunit.runner.console\tools\xunit.console.exe"})
)

Target "Package" (fun _ -> 
    Paket.Pack id
)

Target "PublishNuget" (fun _ ->
    () //Paket.Push id
)

"Clean"
    ==> "BuildSolution"
    ==> "Merge"
    ==> "Test"
    ==> "Package"
    ==> "PublishNuget"

RunTargetOrDefault "BuildSolution"
