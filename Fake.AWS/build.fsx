#r "packages/FAKE/tools/FakeLib.dll" // include Fake lib
open Fake 

let nugetDir = "./nuget"
let buildDir = "./build"

Target "Clean" (fun _ -> CleanDirs [buildDir; nugetDir; ])


Target "BuildSolution" (fun _ ->
    MSBuildWithDefaults "Build" ["./Fake.AWS.sln"]
    |> Log "AppBuild-Output: "
)

Target "PublishNuget" (fun _ ->
    Paket.Pack id
    Paket.Push id
)

"Clean"
    ==> "BuildSolution"
    ==> "PublishNuget"

RunTargetOrDefault "BuildSolution"

