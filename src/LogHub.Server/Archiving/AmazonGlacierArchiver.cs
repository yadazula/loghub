using System.IO;
using Amazon.Glacier.Transfer;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
	public class AmazonGlacierArchiver : AbstractLogArchiver
	{
		public AmazonGlacierArchiver(IDocumentStore documentStore)
			: base(documentStore)
		{
		}

		public override void Archive(Retention retention, string filePath)
		{
			if (!retention.ArchiveToGlacier)
				return;

			var archiveSettings = GetSettings().Archive;
			
			if(IsValid(archiveSettings) == false)
				return;

			var region = Amazon.RegionEndpoint.GetBySystemName(archiveSettings.GlacierRegionName);

			using (var transferManager = new ArchiveTransferManager(archiveSettings.GlacierAccessKey, archiveSettings.GlacierSecretKey, region))
			{
				transferManager.Upload(archiveSettings.GlacierVault, Path.GetFileNameWithoutExtension(filePath), filePath);
			}
		}

		private bool IsValid(Settings.ArchiveSettings archiveSettings)
		{
			if (archiveSettings.GlacierRegionName.IsNullOrWhiteSpace())
				return false;

			if (archiveSettings.GlacierAccessKey.IsNullOrWhiteSpace())
				return false;

			if (archiveSettings.GlacierSecretKey.IsNullOrWhiteSpace())
				return false;

			if (archiveSettings.GlacierVault.IsNullOrWhiteSpace())
				return false;

			return true;
		}
	}
}