using System.IO;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
	public class DiskArchiver : AbstractLogArchiver
	{
		public DiskArchiver(IDocumentStore documentStore)
			: base(documentStore)
		{
		}

		public override void Archive(Retention retention, string filePath)
		{
			if (!retention.ArchiveToDisk)
				return;

			var archiveSettings = GetSettings().Archive;

			if (IsValid(archiveSettings) == false)
				return;

			var fileName = Path.GetFileName(filePath);
			var destFileName = Path.Combine(archiveSettings.DiskPath, fileName);
			File.Copy(filePath, destFileName);
		}

		private bool IsValid(Settings.ArchiveSettings archiveSettings)
		{
			if (archiveSettings.DiskPath.IsNullOrWhiteSpace())
				return false;

			return true;
		}
	}
}