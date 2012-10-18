using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LogHub.Core.Models;
using LogHub.Web.ViewModels;
using LogHub.Core.Extensions;

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
				.ForMember(x => x.EmailTo, o => o.ResolveUsing(m =>
					{
						if (m.EmailToList.IsNull())
							return string.Empty;

						return string.Join(",", m.EmailToList);
					}
					));

			Mapper.CreateMap<LogAlertView, LogAlert>()
				.ForMember(x => x.Minutes, o => o.MapFrom(m => TimeSpan.FromMinutes(m.Minutes)))
				.ForMember(x => x.EmailToList, o => o.ResolveUsing(m =>
					{
						if (m.EmailTo.IsNullOrWhiteSpace())
						{
							return new List<string>();
						}

						var emails = m.EmailTo.Split(',');
						return emails.Select(email => email.Trim()).ToList();
					}));
		}
	}
}