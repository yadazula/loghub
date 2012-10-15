using System;
using System.Collections.Generic;
using System.Linq;
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
				.ForMember(x => x.Date, o => o.MapFrom(m => m.Date.ToString("dd-MM-yyyy HH:mm:ss.fff K")));

			Mapper.CreateMap<UserInput, User>();
			Mapper.CreateMap<Settings, Settings>();

			Mapper.CreateMap<LogAlert, LogAlertView>()
				.ForMember(x => x.Minutes, o => o.MapFrom(m => m.Minutes.TotalMinutes))
				.ForMember(x => x.EmailTo, o => o.MapFrom(m => string.Join(",", m.EmailToList)));

			Mapper.CreateMap<LogAlertView, LogAlert>()
				.ForMember(x => x.Minutes, o => o.MapFrom(m => TimeSpan.FromMinutes(m.Minutes)))
				.ForMember(x => x.EmailToList, o => o.ResolveUsing(m =>
					{
						var emails = m.EmailTo.Split(',');
						return emails.Select(email => email.Trim()).ToList();
					}));
		}
	}
}