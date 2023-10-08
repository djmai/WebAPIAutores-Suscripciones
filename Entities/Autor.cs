using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entities
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        // [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        // [Range(18, 100, ErrorMessage = "El campo {0} debe ser entre {1} y {2}")]
        // [NotMapped]
        // public int Edad { get; set; }

        // [CreditCard(ErrorMessage = "El campo {0} no es un número de tarjeta de crédito válido.")]
        // [NotMapped]
        // public string TarjetaCredito { get; set; }

        // [Url(ErrorMessage = "El campo {0} no es una URL http, https o ftp totalmente calificada y válida.")]
        // [NotMapped]
        // public string URL { get; set; }

        [NotMapped]
        public int Menor { get; set; }

        [NotMapped]
        public int Mayor { get; set; }

        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new String[] { nameof(Nombre) });
                }
            }

            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser más grande que el campo Mayor", new String[] { nameof(Menor) });
            }
        }
    }
}