using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTOs
{
	public class CrearRestriccionesIPDTO
	{
		public int LlaveId { get; set; }

		[Required]
		public string IP { get; set; }
	}
}
