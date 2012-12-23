using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Extensions;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
	public class AmazonS3Archiver : AbstractLogArchiver
	{
		protected override void DoArchive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath)
		{
			using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(archiveSettings.S3AccessKey, archiveSettings.S3SecretKey))
			{
				var putObjectRequest = new PutObjectRequest();
				putObjectRequest.WithFilePath(filePath)
												.WithBucketName(archiveSettings.S3BucketName)
												.WithKey(Path.GetFileNameWithoutExtension(filePath));

				client.PutObject(putObjectRequest);
			}
		}

		protected override bool IsValid(Settings.ArchiveSettings archiveSettings, Retention retention)
		{
			if (!retention.ArchiveToS3)
				return false;

			if (archiveSettings.S3AccessKey.IsNullOrWhiteSpace())
				return false;

			if (archiveSettings.S3SecretKey.IsNullOrWhiteSpace())
				return false;

			if (archiveSettings.S3BucketName.IsNullOrWhiteSpace())
				return false;

			return true;
		}
	}
}