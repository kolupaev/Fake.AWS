#I "packages/FAKE/tools/"
#I "build/merged"

#r "FakeLib.dll"
#r "Fake.AWS.dll"

open Fake
open Fake.AWS.OpsWorks
open Fake.AWS.Profiles
open Fake.AWS.CloudWatch

Fake.AWS.Configuration.setProfileAndEndpoint "FakeTest" "us-east-1"

Target "PrintName" (fun _ -> 
    Fake.AWS.Profiles.printProfileName()
)

Target "DescribeStacks" (fun _ ->
    Fake.AWS.OpsWorks.describeStacks()
)

Target "PrintS3Buckets" (fun _ -> 
    Fake.AWS.S3.printBuckets()
)

Target "CanCompileTheRest" (fun _ -> 
    Fake.AWS.S3.upload "src" "dst" "dst" |> ignore
    
    Fake.AWS.OpsWorks.updateCookbooks "stack" "app" |> ignore
    Fake.AWS.OpsWorks.updateCookbooksAsync "stack" "app" |> ignore
    
    Fake.AWS.OpsWorks.deploy "stack" "app" |> ignore
    Fake.AWS.OpsWorks.deployAsync "stack" "app" |> ignore
)

RunTargetOrDefault "Default"

let asx = Fake.AWS.CloudWatch.alarmspec {
    alarm "foo"
}