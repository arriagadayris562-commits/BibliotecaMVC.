
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            IConfiguration configuration,
            ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Consultando la lista de categorias");

            try
            {
                var categorias = new List<Categoria>();

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                });
                            }
                        }
                    }
                }

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar la lista de categorias");
                throw;
            }
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operacion de creacion de categoria");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando la operacion de creacion de categoria");

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("No se pudo crear la categoria porque el nombre es obligatorio");

                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                _logger.LogInformation("Categoria creada correctamente");
                TempData["SuccessMessage"] = "Categoria guardada correctamente.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoria");
                throw;
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation(
                "Consultando la categoria con identificador {CategoriaId} para edicion", id);

            try
            {
                Categoria categoria = null;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @Id";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id", id);
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = Convert.ToInt32(lector["ID"]),
                                    Nombre = lector["Nombre"].ToString(),
                                    Descripcion = lector["Descripcion"] != DBNull.Value
                                        ? lector["Descripcion"].ToString()
                                        : string.Empty
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontro la categoria con identificador {CategoriaId} para editarla", id);
                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al consultar la categoria con identificador {CategoriaId} para edicion", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            _logger.LogInformation(
                "Iniciando la operacion de edicion de categoria con identificador {CategoriaId}",
                categoria?.ID);

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning(
                    "No se pudo editar la categoria porque el nombre es obligatorio");

                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @Id";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id", categoria.ID);
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);

                        conexion.Open();
                        var filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontro la categoria con identificador {CategoriaId} para actualizarla",
                                categoria.ID);
                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Categoria con identificador {CategoriaId} actualizada correctamente",
                    categoria.ID);

                TempData["SuccessMessage"] = "Categoria actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al editar la categoria con identificador {CategoriaId}", categoria.ID);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Iniciando la operacion de eliminacion de categoria con identificador {CategoriaId}", id);

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @Id";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id", id);

                        conexion.Open();
                        var filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontro la categoria con identificador {CategoriaId} para eliminarla", id);
                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Categoria con identificador {CategoriaId} eliminada correctamente", id);

                TempData["SuccessMessage"] = "Categoria eliminada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al eliminar la categoria con identificador {CategoriaId}", id);
                throw;
            }
        }
    }
}
