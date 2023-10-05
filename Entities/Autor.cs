using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class Autor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        public string Nombre { get; set; }

        [Range(18, 100, ErrorMessage = "El campo {0} debe ser entre {1} y {2}")]
        [NotMapped]
        public int Edad { get; set; }

        [CreditCard(ErrorMessage = "El campo {0} no es un número de tarjeta de crédito válido.")]
        [NotMapped]
        public string TarjetaCredito { get; set; }

        [Url(ErrorMessage = "El campo {0} no es una URL http, https o ftp totalmente calificada y válida.")]
        [NotMapped]
        public string URL { get; set; }

        public List<Libro> Libros { get; set; }
    }
}