using System;
using System.Security.Principal;
using LogHub.Core.Models;

namespace LogHub.Web.Filters
{
  public class LogHubPrincipal : IPrincipal
  {
    private readonly UserRole currentRole;
    public IIdentity Identity { get; private set; }

    public LogHubPrincipal(IIdentity identity, string role)
    {
      currentRole = (UserRole)Enum.Parse(typeof(UserRole), role);
      Identity = identity;
    }

    public bool IsInRole(string role)
    {
      var targetRole = (UserRole)Enum.Parse(typeof(UserRole), role);

      if (targetRole == currentRole)
      {
        return true;
      }

      if (targetRole == UserRole.Reader && currentRole == UserRole.Administrator)
      {
        return true;
      }

      return false;
    }
  }
}