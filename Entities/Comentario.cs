using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class Comentario
    {
        public int Id { get; set; }

        public string Contenido { get; set; }

        public int LibroId { get; set; }

        public Libro Libro { get; set; }

    }
}