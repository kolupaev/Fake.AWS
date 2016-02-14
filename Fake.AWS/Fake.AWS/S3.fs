module Fake.AWS.S3

type ObjectMetadata = {
    ETag: string;
    ContentLength: int64;
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
    }

let upload (src:string) dstBucket dstKey = 
    let s3 = Clients.s3()
    let transfer = new Amazon.S3.Transfer.TransferUtility(s3)
    transfer.Upload(src, dstBucket, dstKey)
    getObjectMetadata dstBucket dstKey

