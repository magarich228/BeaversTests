using Minio;
using Minio.DataModel.Args;

var client = new MinioClient()
    .WithCredentials("kirill", "minio123")
    .WithSSL(false)
    .WithEndpoint("localhost:9000")
    .Build();

await client.MakeBucketAsync(new MakeBucketArgs().WithBucket("test"));

var list = await client.ListBucketsAsync();

Console.WriteLine(list.Buckets.Count);