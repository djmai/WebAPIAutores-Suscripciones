using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebAPIAutores.Entities
{
    public class Comentario
    {
        public int Id { get; set; }

        public string Contenido { get; set; }

        public int LibroId { get; set; }

        public Libro Libro { get; set; }

        public string UsuarioId { get; set; }

        public IdentityUser Usuario { get; set; }

    }
}