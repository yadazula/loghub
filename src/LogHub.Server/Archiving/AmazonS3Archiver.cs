using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class AmazonS3Archiver : AbstractLogArchiver<AmazonS3Setting>
  {
    protected override void Archive(AmazonS3Setting setting, string filePath)
    {
      using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(setting.AWSAccessKey, setting.AWSSecretKey))
      {
        var putObjectRequest = new PutObjectRequest();
        putObjectRequest.WithFilePath(filePath)
                        .WithBucketName(setting.BucketName)
                        .WithKey(Path.GetFileNameWithoutExtension(filePath));

        client.PutObject(putObjectRequest);
      }
    }
  }
}