using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<LibroDTO>>> Get()
        {
            var libros = await context.Libros.ToListAsync();

            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            // var libro = await context.Libros.Include(libroDB => libroDB.Comentarios).FirstOrDefaultAsync(x => x.Id == id);
            var libro = await context.Libros
                .Include(libroBD => libroBD.AutoresLibros)
                .ThenInclude(autorLibroBD => autorLibroBD.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            // var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            // if (!existeAutor)
            //     return BadRequest($"No existe el autor del Id: {libro.AutorId}");

            if (libroCreacionDTO.AutoresIds == null)
                return BadRequest("No hay autores asociados al libro");

            var autoresIds = await context.Autores.Where(autorDB => libroCreacionDTO.AutoresIds.Contains(autorDB.Id)).Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
                return BadRequest("No existe uno de los autores enviados");

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}