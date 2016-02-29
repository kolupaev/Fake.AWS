# Fake.AWS

## What is it

Fake.AWS is a collection of bindings of AWS APIs to the Fake build system.
At this point it is in early stage. It supports minimum set of operations to implement continious deployment via OpsWorks.

## Installation

Modify `build.bat` file to download latest Fake.AWS version

      IF NOT EXISTS "packages\Fake.AWS" (
        ".nuget\NuGet.exe" "Install" "Fake.AWS" "-OutputDirectory" "packages" "-ExcludeVersion" "-Prerelease"
      )

Modify `build.fsx` to reference `Fake.AWS.dll` and configure your profile name and endpoint

    #r "Fake.AWS.dll"
    Fake.AWS.Configuration.setProfileAndEndpoint "FakeTest" "us-east-1" // FakeTest should be a registered visual studio aws profile

## Implemented bindings

### S3
    let metadata = Fake.AWS.S3.upload sourceFile dstBucket dstKey

### OpsWorks
    Fake.AWS.OpsWorks.updateCookbooks "stackId" "appId"
    Fake.AWS.OpsWorks.updateCookbooksAsync "stackId" "appId"

    Fake.AWS.OpsWorks.deploy "stackId" "appId"
    Fake.AWS.OpsWorks.deployAsync "stackId" "appId"

### CodeDeply
	Fake.AWS.CodeDeploy.push "AppName" "path\to\app" "targetBucket" "targetKey"
	Fake.AWS.CodeDeploy.deployAsync "AppName" metadata "deploymentGroup" "deploymentConfig"
	Fake.AWS.CodeDeploy.deploy "AppName" metadata "deploymentGroup" "deploymentConfig"
