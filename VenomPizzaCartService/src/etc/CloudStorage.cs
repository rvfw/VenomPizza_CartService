using Amazon;
using Amazon.S3;

namespace VenomPizzaCartService.src.etc;

public class CloudStorage
{
    private readonly AmazonS3Client _client;
    private const string BucketName = "venomPizza";
    private const string FileKey = "cart-service/products-cache-snapshot.json";
    public CloudStorage()
    {
        AmazonS3Config config = new AmazonS3Config
        {
            ServiceURL = "https://s3.eu-central-003.backblazeb2.com",
            ForcePathStyle = true,
            Timeout=TimeSpan.FromSeconds(30),
        };
        _client = new AmazonS3Client("003f2a0142a0fe70000000001", "K003zbdLzxRc6DjDCAoRHYoT1ZmMgkQ", config);
    }

    public async Task UploadSnapshotAsync(string content)
    {
        var key = FileKey;
        await _client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            ContentBody = content,
            ContentType= "application/json"
        });
    }

    public async Task<string?> ReadSnapshot()
    {
        try
        {
            var responce = await _client.GetObjectAsync(BucketName, FileKey);
            using var streamReader = new StreamReader(responce.ResponseStream);
            return await streamReader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
