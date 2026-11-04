
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorio;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(
            IRepositorioLibro repositorio,
            ILogger<LibrosController> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Consultando la lista de libros");

            try
            {
                var libros = _repositorio.ObtenerTodos();
                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar la lista de libros");
                throw;
            }
        }

         public IActionResult Details(int id)
        {
            _logger.LogInformation(
                "Consultando los detalles del libro con identificador {LibroId}", id);

            try
            {
                var libro = _repositorio.ObtenerPorId(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al consultar los detalles del libro con identificador {LibroId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operación de creación de libro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Create(Libro libro)
        {
            _logger.LogInformation("Iniciando la operación de creación de libro");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("No se pudo crear el libro porque los datos enviados no son válidos");
                return View(libro);
            }

            try
            {
                _repositorio.Agregar(libro);
                _logger.LogInformation("Libro creado correctamente");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el libro");
                throw;
            }
        }

        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando la consulta del libro para edición con identificador {LibroId}", id);

            try
            {
                var libro = _repositorio.ObtenerPorId(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con identificador {LibroId} para editarlo", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al consultar el libro con identificador {LibroId} para editarlo", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Edit(int id, Libro libro)
        {
            _logger.LogInformation("Iniciando la operación de edición del libro con identificador {LibroId}", id);

            if (id != libro.ID)
            {
                _logger.LogWarning("El identificador de la ruta no coincide con el libro enviado: {LibroId}", id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("No se pudo editar el libro porque los datos enviados no son válidos");
                return View(libro);
            }

            try
            {
                var existente = _repositorio.ObtenerPorId(id);

                if (existente == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId} para editarlo", id);
                    return NotFound();
                }

                _repositorio.Actualizar(libro);
                _logger.LogInformation("Libro con identificador {LibroId} actualizado correctamente", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al editar el libro con identificador {LibroId}", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando la operación de eliminación del libro con identificador {LibroId}", id);

            try
            {
                var libro = _repositorio.ObtenerPorId(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con identificador {LibroId} para eliminarlo", id);
                    return NotFound();
                }

                _repositorio.Eliminar(id);

                _logger.LogInformation(
                    "Libro con identificador {LibroId} eliminado correctamente", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al eliminar el libro con identificador {LibroId}", id);
                throw;
            }
        }
    }
}
