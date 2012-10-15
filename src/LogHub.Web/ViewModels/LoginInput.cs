using System.ComponentModel.DataAnnotations;

namespace LogHub.Web.ViewModels
{
	public class LoginInput
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}