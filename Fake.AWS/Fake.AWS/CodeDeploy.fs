module Fake.AWS.CodeDeploy

open Amazon.CodeDeploy
open Clients
open System.IO

module internal Implementation = 
    let getS3Location (metadata:S3.ObjectMetadata) = 
        new Model.S3Location(
            BundleType = BundleType.Zip, 
            Bucket = metadata.Bucket, 
            Key = metadata.Key, 
            ETag = metadata.ETag, 
            Version = metadata.VersionId
        )
    let push appName directory appBuket appKey =
        let client = codedeploy()
        let tempArchive = Path.GetTempFileName();
        try
            File.Delete tempArchive
            Compression.ZipFile.CreateFromDirectory(directory, tempArchive, Compression.CompressionLevel.Optimal, false)
            let metadata = S3.upload tempArchive appBuket appKey
            let revision = new Model.RevisionLocation(
                RevisionType = RevisionLocationType.S3, 
                S3Location = getS3Location metadata
            )
            client.RegisterApplicationRevision(new Model.RegisterApplicationRevisionRequest(ApplicationName = appName, Revision = revision)) |> assertSuccess "Failed to register app version"
            metadata
        finally
            File.Delete tempArchive
    let deploy appName metadata deploymentGroup deploymentConfig = 
        let client = codedeploy()
        let revision = new Model.RevisionLocation(
            RevisionType = RevisionLocationType.S3, 
            S3Location = getS3Location metadata
        )
        let request = new Model.CreateDeploymentRequest(ApplicationName = appName, Revision = revision, DeploymentGroupName = deploymentGroup, DeploymentConfigName = deploymentConfig )
        let response = client.CreateDeployment(request)
        response |> assertSuccess "Failed to create deployment"
        spinWait "CodeDeploy" (fun () -> 
            let deployment = client.GetDeployment(new Model.GetDeploymentRequest(DeploymentId = response.DeploymentId))
            deployment |> assertSuccess "Get deployment info"
            match deployment.DeploymentInfo.Status.Value with
                | "Created" | "Queued" -> InProgress(deployment.DeploymentInfo.Status.Value)
                | "InProgress" -> 
                    let overview = deployment.DeploymentInfo.DeploymentOverview
                    if overview = null then 
                        InProgress("InProgress")
                    else 
                        InProgress(sprintf "InProgress %2d/%2d/%2d" overview.Pending overview.InProgress overview.Succeeded )
                | _ -> Failed(deployment.DeploymentInfo.Status.Value)
        )
        
        
let push appName directory appBuket appKey = Implementation.push appName directory appBuket appKey
let deployAsync appName metadata deploymentGroup deploymentConfig = Implementation.deploy appName metadata deploymentGroup deploymentConfig

let deploy appName metadata deploymentGroup deploymentConfig = Async.RunSynchronously (deployAsync appName metadata deploymentGroup deploymentConfig)
