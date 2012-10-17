using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LogHub.Core.Extensions;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using LogHub.Server.Archiving;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace LogHub.Server.Tasks
{
  public class RetentionScheduledTask : IScheduledTask
  {
    private readonly IDocumentStore documentStore;
    private readonly ILogArchiver[] logArchivers;

    public RetentionScheduledTask(IDocumentStore documentStore, ILogArchiver[] logArchivers)
    {
      this.documentStore = documentStore;
      this.logArchivers = logArchivers;
    }

    public TimeSpan Period
    {
      get { return TimeSpan.FromHours(1); }
    }

    public void Execute()
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var retentions = documentSession.Query<Retention>();

        foreach (var retention in retentions)
        {
          if (retention.NeedsArchiving)
          {
            Archive(retention);
          }

          Delete(retention);
        }
      }
    }

    private void Archive(Retention retention)
    {
      var filePath = Export(retention);
      foreach (var logArchiver in logArchivers)
      {
        logArchiver.Archive(retention, filePath);
      }

      File.Delete(filePath);
    }

    private string Export(Retention retention)
    {
      var filename = retention.Source.IsNullOrWhiteSpace() ? "All-Sources" : GenerateSafeFilename(retention.Source);
      var path = string.Format("{0}.{1}.gz", DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture),
                               filename);

      using (var streamWriter = new StreamWriter(new GZipStream(File.Create(path), CompressionMode.Compress)))
      {
        var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };
        jsonWriter.WriteStartObject();
        jsonWriter.WritePropertyName("Docs");
        jsonWriter.WriteStartArray();
        ExportDocuments(retention, jsonWriter);
        jsonWriter.WriteEndArray();
        jsonWriter.WriteEndObject();
        streamWriter.Flush();
      }

      return Path.Combine(Environment.CurrentDirectory, path);
    }

    private void ExportDocuments(Retention retention, JsonTextWriter jsonWriter)
    {
      var cutoffDate = DateTimeOffset.Now.AddDays(-retention.Days);
      var readMessages = 0;
      while (true)
      {
        using (var documentSession = documentStore.OpenSession())
        {
          var query = documentSession.Query<LogMessage, LogMessage_Search>()
                                     .Where(x => x.Date < cutoffDate);

          if (retention.Source.IsNotNullOrWhiteSpace())
          {
            query = query.Where(x => x.Source == retention.Source);
          }

          var messages = query.Skip(readMessages).Take(1024).ToList();

          if (messages.Count == 0)
          {
            return;
          }

          messages.ForEach(x => RavenJObject.FromObject(x).WriteTo(jsonWriter));
          readMessages += messages.Count;
        }
      }
    }

    private void Delete(Retention retention)
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var cutoffDate = DateTimeOffset.Now.AddDays(-retention.Days);

        var query = documentSession.Query<LogMessage, LogMessage_Search>()
                                   .Where(x => x.Date < cutoffDate);

        if (retention.Source.IsNotNullOrWhiteSpace())
        {
          query = query.Where(x => x.Source == retention.Source);
        }

        documentStore.DatabaseCommands.DeleteByIndex(new LogMessage_Search().IndexName, new IndexQuery { Query = query.ToString() });
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