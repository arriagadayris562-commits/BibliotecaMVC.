using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IAutorService
    {
        /// <summary>
        /// regla de negocio: (cosas obligatorias)-(obtener todos los autores de la base de datos)
        /// </summary>
        /// <returns></returns>
        IEnumerable<Autor> ObtenerTodos();

        /// <summary>
        /// regla de negocio: (cosas obligatorias)-(obtener un autor específico por su ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Autor ObtenerPorId(int id);
    }
}