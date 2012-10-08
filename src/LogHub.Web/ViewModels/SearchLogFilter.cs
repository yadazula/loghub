using System;

namespace LogHub.Web.ViewModels
{
  public class SearchLogFilter : AbstractLogFilter
  {
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
    public string TimeFrom { get; set; }
    public string TimeTo { get; set; }
    public ushort? Page { get; set; }
  }
}