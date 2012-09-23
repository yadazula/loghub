using AutoMapper;
using LogHub.Core.Models;
using LogHub.Web.ViewModels;

namespace LogHub.Web.Infrastructure.AutoMapper
{
  public class AutoMapperConfiguration
  {
    public static void Configure()
    {
      Mapper.CreateMap<LogMessage, LogMessageView>()
            .ForMember(x => x.Date, o => o.MapFrom(m => m.Date.ToString("yyyy-MM-dd HH:mm:ss.fff K")));

      Mapper.CreateMap<UserInput, User>();
    }
  }
}