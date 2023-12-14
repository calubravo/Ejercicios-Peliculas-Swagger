using Microsoft.AspNetCore.Mvc;
using Clase_11_12.Repository;
using Clase_11_12.Domain.Request;
using Microsoft.EntityFrameworkCore;
using Clase_11_12.Domain.Entities;

namespace Clase_11_12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectorController : ControllerBase
    {
        private readonly IMDBContext _context; //inyeccion de dependencia
        public DirectorController(IMDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] GetDirectoresRequest request)
        {  //paginado (trae de a bloques de registros)
            int skip = request.Skip;
            int take = request.Take;
            //var response = new GetDirectoresResponse
            //{
            //    D
            //};

            var result = _context.Directores.Skip(skip).Take(take).ToList();
            int count = _context.Directores.Count();

            return Ok(new { Datos = result, Count = count });
        }
        [HttpGet("{id}")]
        public IActionResult GetDirectorById([FromRoute] int id)
        {
            var result = _context.Directores.Where(w => w.IdDirector == id).FirstOrDefault();
            if (result == null) return NotFound(new { Error = $"No se encuentra el id {id}" });
            return Ok(result);
        }
        [HttpGet("peliculas")]
        public IActionResult GetPeliculasByDirector([FromQuery] int idDirector)
        {
            var result = _context.Directores.Where(w => w.IdDirector == idDirector)
                                           .Include(i => i.Peliculas)
                                           .ToList();
            if (result == null) return NotFound(new { Error = $"No se encuentra el id {idDirector}" });
            return Ok(result);
        }
        //actores y sus actuaciones
        [HttpGet("Actores_Personajes")]
        public IActionResult GetActuacionesDeActor([FromQuery] int idActor)
        {
            var result = _context.Actores
                          .Where(w => w.IdActor == idActor)
                          .SelectMany(a => a.Actuaciones
                              .Select(actuacion => new
                              {
                                  a.IdActor,
                                  NombreActor = a.Nombre,
                                  actuacion.IdActuacion,
                                  actuacion.Papel,
                                  TituloPelicula = actuacion.IdPeliculaNavigation.Titulo
                              })
                          )
                    .ToList();



            if (result == null) return NotFound(new { Error = $"El actor {idActor} no tiene personajes" });
            return Ok(result);
        }
      // actuaciones dentro de una pelicula
        [HttpGet("Actuaciones_Pelicula")]
        public IActionResult GetActuacionesDePelicula ([FromQuery] int idPelicula )
        {
            var result = _context.Peliculas.Where(w => w.IdPelicula == idPelicula)
                                         .Include(i => i.Actuaciones)
                                         .ToList();
            if (result == null) return NotFound(new { Error = $"La pelicula {idPelicula} no se encontro" });
            return Ok(result);
        }
        //productoras sin peliculas
        [HttpGet("Productoras")]
        public IActionResult GetProductorasSinPeli([FromQuery] int idProductora)
        {
            var result = _context.Productoras.Where(w => !w.Peliculas.Any() && w.IdProductora == idProductora)
                                              .FirstOrDefault();

            if (result == null) return NotFound(new { Error = $"La productora de id: {idProductora} ya tiene peliculas asignadas " });

                 return Ok(result);
        }

        
}
}
