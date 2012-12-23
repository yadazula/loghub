using System.IO;
using Amazon.Glacier.Transfer;
using LogHub.Core.Extensions;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
	public class AmazonGlacierArchiver : AbstractLogArchiver
	{
		protected override void DoArchive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath)
		{
			var region = Amazon.RegionEndpoint.GetBySystemName(archiveSettings.GlacierRegionName);

			using (var transferManager = new ArchiveTransferManager(archiveSettings.GlacierAccessKey, archiveSettings.GlacierSecretKey, region))
			{
				transferManager.Upload(archiveSettings.GlacierVault, Path.GetFileNameWithoutExtension(filePath), filePath);
			}
		}

		protected override bool IsValid(Settings.ArchiveSettings archiveSettings, Retention retention)
		{
			if (!retention.ArchiveToGlacier)
				return false;

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