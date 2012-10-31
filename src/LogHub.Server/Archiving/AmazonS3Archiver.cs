using System.IO;
using Amazon.S3.Model;
using LogHub.Core.Extensions;
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

			var archiveSettings = GetSettings().Archive;

			if (IsValid(archiveSettings) == false)
				return;

			using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(archiveSettings.S3AccessKey, archiveSettings.S3SecretKey))
			{
				var putObjectRequest = new PutObjectRequest();
				putObjectRequest.WithFilePath(filePath)
												.WithBucketName(archiveSettings.S3BucketName)
												.WithKey(Path.GetFileNameWithoutExtension(filePath));

				client.PutObject(putObjectRequest);
			}
		}

		private bool IsValid(Settings.ArchiveSettings archiveSettings)
		{
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