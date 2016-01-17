module OpsWorks
open Amazon.OpsWorks
open Clients

module Implementation = 
    type WaitResult =
        | InProgress of status : string
        | Failed of status : string
        | Completed of status : string

    let rec private wait operationName callback =
        async {
                do! Async.Sleep 1000
                let result = callback()
                match result with
                | InProgress(status) ->
                    printf "Operation %s is in progress: %s" operationName status
                    return! wait operationName callback
                | Failed(status) ->
                    printf "Operation %s failed: %s" operationName status
                    return result
                | Completed(status) ->
                    printf "Operation %s completed: %s" operationName status
                    return result
            }
    let private deploymentStatus (client : Amazon.OpsWorks.AmazonOpsWorksClient) deploymentId =
        let response = client.DescribeDeployments(Model.DescribeDeploymentsRequest(DeploymentIds = ResizeArray<string>([deploymentId])))
        if response.Deployments.Count = 0 then
            Failed("Deployment not found")
        else
            let deployment = response.Deployments.[0]
            match deployment.Status with
            | "running" ->
                InProgress(deployment.Status)
            | "successful" -> Completed(deployment.Status)
            | _ -> Failed(deployment.Status)

    let internal runDeployCommand command stack app =
        let client = opsworks()
        let request = Model.CreateDeploymentRequest(AppId = app, StackId = stack, Command = command)
        let response = client.CreateDeployment(request)
        Async.RunSynchronously (wait command.Name.Value (fun () -> deploymentStatus client response.DeploymentId))


let deploy stack app = Implementation.runDeployCommand (Model.DeploymentCommand(Name = DeploymentCommandName.Deploy))
let updateCookbooks stack app = Implementation.runDeployCommand (Model.DeploymentCommand(Name = DeploymentCommandName.Update_custom_cookbooks))