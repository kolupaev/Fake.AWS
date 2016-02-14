module Fake.AWS.Clients

let internal opsworks() = new Amazon.OpsWorks.AmazonOpsWorksClient(Configuration.credentials, Configuration.region)