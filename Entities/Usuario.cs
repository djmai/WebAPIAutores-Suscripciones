using Microsoft.AspNetCore.Identity;

namespace WebAPIAutores.Entities
{
	public class Usuario : IdentityUser
	{
		public bool MalaPaga { get; set; }
	}
}
