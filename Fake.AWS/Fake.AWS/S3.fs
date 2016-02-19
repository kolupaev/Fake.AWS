module Fake.AWS.S3
open Amazon.S3.Transfer;

type ObjectMetadata = {
    ETag: string;
    ContentLength: int64;
    VersionId: string; 
    Bucket: string;
    Key: string
}

let printBuckets () = 
    let s3 = Clients.s3()
    s3.ListBuckets().Buckets
        |> Seq.iter (fun b -> printf "Bucket: %s" b.BucketName)


let getObjectMetadata bucket key = 
    let info = Clients.s3().GetObjectMetadata(bucket, key)
    {
        ETag = info.ETag;
        ContentLength = info.ContentLength;
        VersionId = info.VersionId; 
        Bucket = bucket; 
        Key = key
    }

let upload src dstBucket dstKey = 
    let s3 = Clients.s3()
    let transfer = new TransferUtility(s3)
    let request = new TransferUtilityUploadRequest(FilePath = src, BucketName = dstBucket, Key = dstKey)
    request.UploadProgressEvent.Add(fun p -> printf "Uploading %s: %3d%%\n" dstKey p.PercentDone) 
    transfer.Upload(request)
    getObjectMetadata dstBucket dstKey

