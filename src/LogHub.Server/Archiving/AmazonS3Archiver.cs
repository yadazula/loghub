using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public class AmazonS3Archiver : AbstractLogArchiver
  {
    public AmazonS3Archiver(IDocumentStore documentStore)
      : base(documentStore)
    {
    }

    public override void Archive(Retention retention, string filePath)
    {
      if (!retention.ArchiveToS3)
        return;

      var settings = GetSettings();
      using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(settings.Archive.S3AccessKey, settings.Archive.S3SecretKey))
      {
        var putObjectRequest = new PutObjectRequest();
        putObjectRequest.WithFilePath(filePath)
                        .WithBucketName(settings.Archive.S3BucketName)
                        .WithKey(Path.GetFileNameWithoutExtension(filePath));

        client.PutObject(putObjectRequest);
      }
    }
  }
}