using System.IO;
using Amazon.Glacier.Transfer;
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

			var settings = GetSettings();
			using (
				var transferManager = new ArchiveTransferManager(settings.Archive.GlacierAccessKey,
				                                                 settings.Archive.GlacierSecretKey,
				                                                 Amazon.RegionEndpoint.GetBySystemName(
					                                                 settings.Archive.GlacierRegionName)))
			{
				transferManager.Upload(settings.Archive.GlacierVault, Path.GetFileNameWithoutExtension(filePath), filePath);
			}
		}
	}
}