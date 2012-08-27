﻿using System.Collections.Generic;

namespace LogHub.Web.Models
{
  public class PagedView<T>
  {
    public ushort Page { get; set; }
    public ushort PerPage { get; set; }
    public int Total { get; set; }
    public IEnumerable<T> Models { get; set; }
  }
}