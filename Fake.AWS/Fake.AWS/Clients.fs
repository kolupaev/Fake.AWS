module Clients

let private configureCredentialsStore() =
    let userProfileFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
    let existingConfigFiles = ["credentials"; "config"] |> Seq.map (sprintf "%s\.aws\%s" userProfileFolder) |> Seq.filter (System.IO.File.Exists)
    match Seq.tryHead existingConfigFiles with
    | Some(config) -> Amazon.AWSConfigs.AWSProfilesLocation <- config
    | None -> ()

configureCredentialsStore()

let internal opsworks() = new Amazon.OpsWorks.AmazonOpsWorksClient()