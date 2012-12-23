using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
	public interface ILogArchiver
	{
		void Archive(Settings.ArchiveSettings archiveSettings, Retention retention, string filePath);
	}
}