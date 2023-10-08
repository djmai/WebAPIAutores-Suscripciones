using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entities
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int AutorId { get; set; }

        public Autor Autor { get; set; }
    }
}