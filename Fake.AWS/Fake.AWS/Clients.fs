module Fake.AWS.Clients

let internal opsworks() = new Amazon.OpsWorks.AmazonOpsWorksClient(Configuration.credentials, Configuration.region)
let internal s3() = new Amazon.S3.AmazonS3Client(Configuration.credentials, Configuration.region)
let internal cloudwatch() = new Amazon.CloudWatch.AmazonCloudWatchClient(Configuration.credentials, Configuration.region)
let internal codedeploy() = new Amazon.CodeDeploy.AmazonCodeDeployClient(Configuration.credentials, Configuration.region)

type WaitResult =
    | InProgress of status : string
    | Failed of status : string
    | Completed of status : string

let internal assertSuccess failMessage (response: Amazon.Runtime.AmazonWebServiceResponse) = 
    let format() = 
        response.ResponseMetadata.Metadata 
            |> Seq.map((fun i -> sprintf "%s:%s" i.Key i.Value))
            |> String.concat "; "
    if response.HttpStatusCode <> System.Net.HttpStatusCode.OK then
        failwith (sprintf "%s: %s" failMessage (format()))

let rec internal spinWait operationName callback =
    async {
            do! Async.Sleep 1000
            let result = callback()
            match result with
            | InProgress(status) ->
                printf "Operation %s is in progress: %s\n" operationName status
                return! spinWait operationName callback
            | Failed(status) ->
                printf "Operation %s failed: %s\n" operationName status
                return result
            | Completed(status) ->
                printf "Operation %s completed: %s\n" operationName status
                return result
        }