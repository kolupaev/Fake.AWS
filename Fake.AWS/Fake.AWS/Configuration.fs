module Fake.AWS.Configuration

let mutable internal credentials = new Amazon.Runtime.StoredProfileAWSCredentials()
let mutable internal region = Amazon.RegionEndpoint.USEast1

let setProfileAndEndpoint name endpoint = 
    Amazon.Runtime.StoredProfileAWSCredentials.CanCreateFrom(name, null) |> ignore
    credentials <- new Amazon.Runtime.StoredProfileAWSCredentials(name)
    region <- Amazon.RegionEndpoint.GetBySystemName(endpoint)
