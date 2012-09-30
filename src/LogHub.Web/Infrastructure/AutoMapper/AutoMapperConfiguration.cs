using System;
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
      Mapper.CreateMap<Settings, Settings>();

      Mapper.CreateMap<LogAlert, LogAlertView>()
            .ForMember(x => x.Minutes, o => o.MapFrom(m => m.Minutes.TotalMinutes));

      Mapper.CreateMap<LogAlertView, LogAlert>()
      .ForMember(x => x.Minutes, o => o.MapFrom(m => TimeSpan.FromMinutes(m.Minutes)));
    }
  }
}