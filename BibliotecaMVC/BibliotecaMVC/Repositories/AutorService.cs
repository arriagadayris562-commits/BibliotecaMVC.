using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class AutorService : IAutorService
    {
        public IEnumerable<Autor> ObtenerTodos()
        {
            return new List<Autor>()
            {
                new Autor
                {
                    ID = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "mexico",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    Activo = true
                },
                new Autor
                {
                    ID = 2,
                    Nombre = "Isabel",
                    Apellido = "Mancia",
                    Nacionalidad = "España",
                    FechaNacimiento = new DateTime(1942, 8, 2),
                    Activo = true
                },
                new Autor
                {
                    ID = 3,
                    Nombre = "Jorge Luis",
                    Apellido = "Juan",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = new DateTime(1899, 8, 24),
                    Activo = false
                },
                new Autor
                {
                    ID = 4,
                    Nombre = "Mario",
                    Apellido = "Vargas",
                    Nacionalidad = "Peruano",
                    FechaNacimiento = new DateTime(1936, 3, 28),
                    Activo = true
                },
                new Autor
                {
                    ID = 5,
                    Nombre = "Julio",
                    Apellido = "Mendozo",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = new DateTime(1914, 8, 26),
                    Activo = false
                },
                 new Autor
                {
                    ID = 6,
                    Nombre = "Nuevo",
                    Apellido = "Nuevo Autor",
                    Nacionalidad = "Salvadoreño",
                    FechaNacimiento = new DateTime(2000, 8, 26),
                    Activo = true
                }
            };
        }

        public Autor ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(a => a.ID == id);
        }
    }
}