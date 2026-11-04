
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioLibro : IRepositorioLibro
    {
        private readonly BibliotecaContext _context;

        public RepositorioLibro(BibliotecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Libro> ObtenerTodos()
        {
            return _context.Libros.ToList();
        }

        public Libro ObtenerPorId(int id)
        {
            return _context.Libros.Find(id);
        }

        public void Agregar(Libro libro)
        {
            _context.Libros.Add(libro);
            _context.SaveChanges();
        }

        public void Actualizar(Libro libro)
        {
            var libroExistente = _context.Libros.Find(libro.ID);

            if (libroExistente != null)
            {
                _context.Entry(libroExistente).CurrentValues.SetValues(libro);
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var libro = _context.Libros.Find(id);

            if (libro != null)
            {
                _context.Libros.Remove(libro);
                _context.SaveChanges();
            }
        }
    }
}
