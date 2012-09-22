using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.Infrastructure.Common;
using LogHub.Web.Models;
using LogHub.Web.ViewModels;
using Raven.Client;

namespace LogHub.Web.Controllers
{
  public class UsersController : AbstractApiController
  {
    public UsersController(IDocumentSession documentSession)
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
      var user = DocumentSession.Query<User>()
                                .FirstOrDefault(x => x.Username == userInput.Username);

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
      var user = DocumentSession.Query<User>()
                                .FirstOrDefault(x => x.Username == userInput.Username);

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
      if (username == Models.User.UndeletableAdminUser)
      {
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      }

      var user = DocumentSession.Query<User>()
                                .Single(x => x.Username == username);

      DocumentSession.Delete(user);
    }
  }
}