﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using LogHub.Web.Models;

namespace LogHub.Web.Controllers
{
  public class RecentController : ApiController
  {
    public IEnumerable<LogMessageView> Get([FromUri]RecentLogFilter recentLogFilter)
    {
      for (var i = 1; i <= 5; i++)
      {
        yield return new LogMessageView
        {
          Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
          Host = "localhost",
          Source = "loghub",
          Logger = "SomeLogger",
          Level = LogLevel.Info.ToString(),
          Message = "Made a dictionary database connection with backend pid 2790 and dsn dbname=ddsreminder_dev user=ddsreminder password=xxxxxxxxxxx host=localhost port=5432"
        };
      }
    }
  }
}