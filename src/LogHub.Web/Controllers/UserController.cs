using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.Filters;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.Infrastructure.Common;
using LogHub.Web.ViewModels;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	[ApiAuthorize(Roles = "Administrator")]
	public class UserController : AbstractApiController
	{
		public UserController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public IEnumerable<User> Get()
		{
			var users = DocumentSession.Query<User>()
																 .OrderBy(x => x.Username)
																 .ToList();

			return users;
		}

		public void Post(UserInput userInput)
		{
			var user = DocumentSession.GetUserByUsername(userInput.Username);

			if (user.IsNotNull())
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username is used by another user."));
			}

			user = userInput.MapTo<User>();
			user.SetPassword(userInput.Password);
			DocumentSession.Store(user);
		}

		public void Put(UserInput userInput)
		{
			var user = DocumentSession.GetUserByUsername(userInput.Username);

			if (user.IsNull())
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			userInput.MapPropertiesToInstance(user);

			if (userInput.Password.IsNotNullOrWhiteSpace())
			{
				user.SetPassword(userInput.Password);
			}
		}

		public void Delete(string username)
		{
			if (username == Core.Models.User.Admin)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Can not delete admin user !"));
			}

			var user = DocumentSession.GetUserByUsername(username);

			DocumentSession.Delete(user);
		}
	}
}