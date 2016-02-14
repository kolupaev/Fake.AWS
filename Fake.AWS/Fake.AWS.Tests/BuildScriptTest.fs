namespace Fake.AWS.Tests

module public ``FAKE Integration Test`` =
    open Xunit
    open FsUnit.Xunit

    let ExecuteAndGetOutput program args =
        let proc = new System.Diagnostics.Process()
        proc.StartInfo.FileName <- program
        proc.StartInfo.WorkingDirectory <- ".\\merged"
        proc.StartInfo.Arguments <- args
        proc.StartInfo.RedirectStandardOutput <- true
        proc.StartInfo.UseShellExecute <- false
        proc.Start() |> ignore
        proc.WaitForExit();
        proc.StandardOutput.ReadToEnd()

    type ``BuildScript`` ()=
        [<Fact>]
        member x.``When executed will print profile name`` () =
            let output = ExecuteAndGetOutput "..\\packages\\FAKE\\tools\\FAKE.exe" "../example_build.fsx PrintName"
            Assert.Matches("Profile name: \w+", output)
        [<Fact>]
        member x.``When executed will print OpsWorks stacks`` () =
            let output = ExecuteAndGetOutput "..\\packages\\FAKE\\tools\\FAKE.exe" "../example_build.fsx DescribeStacks"
            Assert.Matches("Stack: \w+", output)
        [<Fact>]
        member x.``When executed will print S3 buckets``() =
            let output = ExecuteAndGetOutput "..\\packages\\FAKE\\tools\\FAKE.exe" "../example_build.fsx PrintS3Buckets"
            Assert.Matches("Bucket: \w+", output)
