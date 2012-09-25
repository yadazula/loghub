using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using LogHub.Server.Archiving;
using NLog;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Json.Linq;

namespace LogHub.Server.Tasks
{
  public class RetentionScheduledTask : IScheduledTask
  {
    private readonly IDocumentStore documentStore;
    private readonly Func<IArchiveSetting, ILogArchiver> archiverFactory;

    public RetentionScheduledTask(IDocumentStore documentStore, Func<IArchiveSetting, ILogArchiver> archiverFactory)
    {
      this.documentStore = documentStore;
      this.archiverFactory = archiverFactory;
    }

    public TimeSpan Period
    {
      get { return TimeSpan.FromHours(1); }
    }

    public void Execute()
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var retentionSettings = documentSession.Query<RetentionSetting>();

        foreach (var retentionSetting in retentionSettings)
        {
          if (retentionSetting.ArchiveSettings.Any())
          {
            Archive(retentionSetting);
          }
          
          Delete(retentionSetting);
        }
      }
    }

    private void Archive(RetentionSetting retentionSetting)
    {
      var filePath = Export(retentionSetting);
      foreach (var archiveSetting in retentionSetting.ArchiveSettings)
      {
        var archiver = archiverFactory(archiveSetting);
        archiver.Archive(archiveSetting, filePath);
      }

      File.Delete(filePath);
    }

    private string Export(RetentionSetting retentionSetting)
    {
      var filename = GenerateSafeFilename(retentionSetting.Source);
      var path = string.Format("{0}.{1}.gz", DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture), filename);

      using (var streamWriter = new StreamWriter(new GZipStream(File.Create(path), CompressionMode.Compress)))
      {
        var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };
        jsonWriter.WriteStartObject();
        jsonWriter.WritePropertyName("Docs");
        jsonWriter.WriteStartArray();
        ExportDocuments(retentionSetting, jsonWriter);
        jsonWriter.WriteEndArray();
        jsonWriter.WriteEndObject();
        streamWriter.Flush();
      }

      return Path.Combine(Environment.CurrentDirectory, path);
    }

    private void ExportDocuments(RetentionSetting retentionSetting, JsonTextWriter jsonWriter)
    {
      var cutoffDate = DateTimeOffset.Now.AddDays(-retentionSetting.Days);
      var readMessages = 0;
      while (true)
      {
        using (var documentSession = documentStore.OpenSession())
        {
          var messages = documentSession.Query<LogMessage, LogMessage_Search>()
                                        .Where(x => x.Source == retentionSetting.Source && x.Date < cutoffDate)
                                        .Skip(readMessages)
                                        .Take(1024)
                                        .ToList();

          if (messages.Count == 0)
          {
            return;
          }

          messages.ForEach(x => RavenJObject.FromObject(x).WriteTo(jsonWriter));
          readMessages += messages.Count;
        }
      }
    }

    private void Delete(RetentionSetting retentionSetting)
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var cutoff = DateTimeOffset.Now.AddDays(-retentionSetting.Days);
        var query = documentSession.Query<LogMessage, LogMessage_Search>()
                                   .Where(x => x.Source == retentionSetting.Source && x.Date < cutoff)
                                   .ToString();

        documentStore.DatabaseCommands.DeleteByIndex(new LogMessage_Search().IndexName, new IndexQuery { Query = query });
      }
    }

    private static string GenerateSafeFilename(string filename)
    {
      foreach (var c in Path.GetInvalidFileNameChars())
      {
        filename = filename.Replace(c, '_');
      }

      return filename;
    }
  }
}