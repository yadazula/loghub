using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.Filters;
using LogHub.Web.Infrastructure.AutoMapper;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	[ApiAuthorize(Roles = "Administrator")]
	public class SettingsController : AbstractApiController
	{
		public SettingsController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public Settings Get()
		{
			var settings = DocumentSession.Query<Settings>().SingleOrDefault() ?? new Settings();
			return settings;
		}

		public void Post(Settings posted)
		{
			var settings = DocumentSession.Query<Settings>().SingleOrDefault();

			if (settings.IsNull())
			{
				settings = posted;				
			}
			else
			{
				posted.MapPropertiesToInstance(settings);
			}

			settings.CreatedBy = User.Identity.Name;
			settings.CreatedAt = DateTimeOffset.Now;
			DocumentSession.Store(settings);
		}
	}
}