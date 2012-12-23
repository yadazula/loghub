using System.IO.Fakes;
using LogHub.Core.Models;
using LogHub.Server.Archiving;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace LogHub.Tests.Server.Archiving
{
	[TestClass]
	public class DiskArchiverTest
	{
		[TestMethod]
		public void Archive_should_copy_to_given_path()
		{
			using (ShimsContext.Create())
			{
				var archiveSettings = new Settings.ArchiveSettings { DiskPath = "c:\\" };
				var retention = new Retention();
				const string filePath = "c:\\foo.2012-01-01-12-00.gz";

				bool isCopyCalled = false;
				ShimFile.CopyStringString = (s, s1) => isCopyCalled = true;

				var diskArchiverMock = new Mock<DiskArchiver> { CallBase = true };
				diskArchiverMock.Protected().Setup<bool>("IsValid", archiveSettings, retention).Returns(true);

				var diskArchiver = diskArchiverMock.Object;
				diskArchiver.Archive(archiveSettings, retention, filePath);

				Assert.IsTrue(isCopyCalled);
			}
		}
	}
}