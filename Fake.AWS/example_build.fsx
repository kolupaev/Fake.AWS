#I "packages/FAKE/tools/"
#I "build/merged"

#r "FakeLib.dll"
#r "Fake.AWS.dll"

open Fake
open Fake.AWS.OpsWorks
open Fake.AWS.Profiles

Fake.AWS.Configuration.setProfileAndEndpoint "FakeTest" "us-east-1"

Target "PrintName" (fun _ -> 
    Fake.AWS.Profiles.printProfileName()
)

Target "DescribeStacks" (fun _ ->
    Fake.AWS.OpsWorks.describeStacks()
)

RunTargetOrDefault "Default"