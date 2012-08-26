using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace LogHub.Web.Views
{
  public static class HtmlHelperExtension
  {
    private static string Templates = null;

    public static IHtmlString RenderTemplates(this HtmlHelper htmlHelper, string path)
    {
      //if(Templates == null)
      //{
      var files = Directory.EnumerateFiles(HttpContext.Current.Server.MapPath(path));
      var stringBuilder = new StringBuilder();

      foreach (var file in files)
      {
        var text = File.ReadAllText(file);
        var templateName = Path.GetFileNameWithoutExtension(file);
        var templateScriptTag = string.Format(@"<script type='text/template' id='{0}-template'>", templateName);
        stringBuilder.AppendLine(templateScriptTag);
        stringBuilder.Append(text);
        stringBuilder.AppendLine("</script>");
      }

      Templates = stringBuilder.ToString();
      //}

      return htmlHelper.Raw(Templates);
    }
  }
}