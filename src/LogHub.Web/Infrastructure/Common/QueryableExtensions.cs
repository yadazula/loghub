using System;
using System.Globalization;
using System.Linq;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.ViewModels;

namespace LogHub.Web.Infrastructure.Common
{
  public static class QueryableExtensions
  {
    public static IQueryable<LogMessage> FilterBy(this IQueryable<LogMessage> query, SearchLogFilter filter)
    {
      DateTime dateFrom;
      if (TryParseDate(filter.DateFrom, filter.TimeFrom, out dateFrom))
      {
        query = query.Where(x => x.Date >= dateFrom);
      }

      DateTime dateTo;
      if (TryParseDate(filter.DateTo, filter.TimeTo, out dateTo))
      {
        query = query.Where(x => x.Date <= dateTo);
      }

      return query.FilterBy(filter as AbstractLogFilter);
    }

    private static bool TryParseDate(string datePart, string timePart, out DateTime dateTime)
    {
      if (datePart.IsNotNullOrWhiteSpace())
      {
        if (timePart.IsNullOrWhiteSpace())
        {
          timePart = "00:00";
        }

        var dateString = string.Format("{0} {1}", datePart, timePart);
        return DateTime.TryParseExact(dateString, "dd-MM-yyyy HH:mm", null, DateTimeStyles.None, out dateTime);
      }

      dateTime = DateTime.MinValue;
      return false;
    }

    public static IQueryable<LogMessage> FilterBy(this IQueryable<LogMessage> query, AbstractLogFilter filter)
    {
      if (filter.Host.IsNotNullOrWhiteSpace())
      {
        query = query.Where(x => x.Host.StartsWith(filter.Host));
      }

      if (filter.Source.IsNotNullOrWhiteSpace())
      {
        query = query.Where(x => x.Source.StartsWith(filter.Source));
      }

      if (filter.Message.IsNotNullOrWhiteSpace())
      {
        query = query.Where(x => x.Message.StartsWith(filter.Message));
      }

      if (filter.Level != LogLevel.None)
      {
        query = query.Where(x => x.Level >= filter.Level);
      }

      return query;
    }

    public static IQueryable<T> Paging<T>(this IQueryable<T> query, ushort? currentPage = 1, int? pageSize = 20)
    {
      return query.Skip((currentPage.Value - 1) * pageSize.Value)
                  .Take(pageSize.Value);
    }
  }
}