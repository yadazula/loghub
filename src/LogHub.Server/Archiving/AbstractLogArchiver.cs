using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
	public abstract class AbstractLogArchiver : ILogArchiver
	{
		public void Archive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath)
		{
			if (IsValid(archiveSettings, retention) == false)
				return;

			DoArchive(archiveSettings, retention, filePath);
		}

		protected abstract bool IsValid(Settings.ArchiveSettings archiveSettings, Retention retention);
		protected abstract void DoArchive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath);
	}
}