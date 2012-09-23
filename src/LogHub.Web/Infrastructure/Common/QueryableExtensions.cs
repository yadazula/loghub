using System.Linq;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.ViewModels;

namespace LogHub.Web.Infrastructure.Common
{
  public static class QueryableExtensions
  {
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

    public static IQueryable<T> Paging<T>(this IQueryable<T> query, ushort? currentPage = null, int? pageSize = null)
    {
      var page = currentPage ?? 1;
      var size = pageSize ?? 20;

      return query.Skip((page - 1) * size)
                  .Take(size);
    }
  }
}