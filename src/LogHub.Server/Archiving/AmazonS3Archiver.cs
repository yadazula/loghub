using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public class AmazonS3Archiver : AbstractLogArchiver
  {
    public AmazonS3Archiver(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public override void Archive(Retention retention, string filePath)
    {
      if (!retention.ArchiveToS3)
        return;

      using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(Settings.Archive.S3AccessKey, Settings.Archive.S3SecretKey))
      {
        var putObjectRequest = new PutObjectRequest();
        putObjectRequest.WithFilePath(filePath)
                        .WithBucketName(Settings.Archive.S3BucketName)
                        .WithKey(Path.GetFileNameWithoutExtension(filePath));

        client.PutObject(putObjectRequest);
      }
    }
  }
}