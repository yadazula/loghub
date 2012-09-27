using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class AmazonS3Archiver : ILogArchiver
  {
    public void Archive(RetentionSetting retentionSetting, ArchiveSettings setting, string filePath)
    {
      if (!retentionSetting.ArchiveToS3)
        return;

      using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(setting.S3AWSAccessKey, setting.S3AWSSecretKey))
      {
        var putObjectRequest = new PutObjectRequest();
        putObjectRequest.WithFilePath(filePath)
                        .WithBucketName(setting.S3BucketName)
                        .WithKey(Path.GetFileNameWithoutExtension(filePath));

        client.PutObject(putObjectRequest);
      }
    }
  }
}