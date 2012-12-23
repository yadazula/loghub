using System.IO;
using LogHub.Core.Extensions;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
	public class DiskArchiver : AbstractLogArchiver
	{
		protected override void DoArchive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath)
		{
			var fileName = Path.GetFileName(filePath);
			var destFileName = Path.Combine(archiveSettings.DiskPath, fileName);
			File.Copy(filePath, destFileName);
		}

		protected override bool IsValid(Settings.ArchiveSettings archiveSettings, Retention retention)
		{
			if (!retention.ArchiveToDisk)
				return false;

			if (archiveSettings.DiskPath.IsNullOrWhiteSpace())
				return false;

			return true;
		}
	}
}