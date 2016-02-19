module Fake.AWS.OpsWorks
open Amazon.OpsWorks
open Clients


module internal Implementation = 
    
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

    let internal runDeployCommandAsync command stack app =
        let client = opsworks()
        let request = Model.CreateDeploymentRequest(AppId = app, StackId = stack, Command = command)
        let response = client.CreateDeployment(request)
        response |> assertSuccess "Failed to create a deployment"
        spinWait command.Name.Value (fun () -> deploymentStatus client response.DeploymentId)
    
    let internal runDeployCommandSync command stack app =
        Async.RunSynchronously (runDeployCommandAsync command stack app)

let deployAsync stack app = Implementation.runDeployCommandAsync (Model.DeploymentCommand(Name = DeploymentCommandName.Deploy)) stack app
let updateCookbooksAsync stack app = Implementation.runDeployCommandAsync (Model.DeploymentCommand(Name = DeploymentCommandName.Update_custom_cookbooks)) stack app

let deploy stack app = Async.RunSynchronously (deployAsync stack app)
let updateCookbooks stack app =  Async.RunSynchronously (updateCookbooksAsync stack app)

let describeStacks() =
    let stacks = opsworks().DescribeStacks()
    stacks.Stacks |> Seq.iter (fun s -> printf "Stack: %s %s" s.Name s.StackId)
    
