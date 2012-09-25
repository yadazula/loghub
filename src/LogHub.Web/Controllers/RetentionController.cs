using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LogHub.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Web.Controllers
{
  public class RetentionController : AbstractApiController
  {
    public RetentionController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public IEnumerable<RetentionSetting> Get()
    {
      var items = DocumentSession.Query<RetentionSetting>()
                                 .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                                 .Include(x => x.CreatedBy)
                                 .ToList();

      foreach (var item in items)
      {
        DocumentSession.Advanced.Evict(item);
        item.CreatedBy = DocumentSession.Load<User>(item.CreatedBy).Name;
      }

      return items;
    }

    public void Post(RetentionSetting retentionSetting)
    {
      var user = DocumentSession.Query<User>().Single(x => x.Username == User.Identity.Name);
      retentionSetting.CreatedBy = DocumentSession.Advanced.GetDocumentId(user);
      retentionSetting.CreatedAt = DateTimeOffset.Now;
      //retentionSetting.ArchiveSettings.Add(new DiskArchiveSetting { Path = @"C:\Users\gokhandemir\Desktop" });
      //retentionSetting.ArchiveSettings.Add(new AmazonS3Setting { AWSAccessKey = "AKIAIXQSV4VZIIW4MPLA", AWSSecretKey = "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", BucketName = "YadazulaBucket" });
      //retentionSetting.ArchiveSettings.Add(new AmazonGlacierArchiveSetting { AWSAccessKey = "AKIAIXQSV4VZIIW4MPLA", AWSSecretKey = "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", Vault = "loghub", RegionName = "us-east-1" });
      DocumentSession.Store(retentionSetting);
    }

    public void Delete(string id)
    {
      var item = DocumentSession.Load<RetentionSetting>(id);
      DocumentSession.Delete(item);
    }
  }
}