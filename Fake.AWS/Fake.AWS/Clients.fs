module Fake.AWS.Clients

let internal opsworks() = new Amazon.OpsWorks.AmazonOpsWorksClient(Configuration.credentials, Configuration.region)
let internal s3() = new Amazon.S3.AmazonS3Client(Configuration.credentials, Configuration.region)
