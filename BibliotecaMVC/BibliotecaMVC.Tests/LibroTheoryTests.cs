using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Tests;

public class LibroTheoryTests
{
    [Theory]
    // Cada [InlineData] es un caso de prueba distinto.
    [InlineData(0)]     
    [InlineData(-1)]    
    [InlineData(-10)]  
    // Comprueba que un precio menor o igual a cero NO sea válido.
    public void PrecioMenorOIgualACero_NoEsValido(int precio)
    {
        // Crea un libro válido y le asigna el precio bajo prueba.
        var libro = CrearLibro();
        libro.Precio = precio;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a la propiedad Precio.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Precio)));
    }

    [Theory]
    [InlineData(1)]     
    [InlineData(15)]
    [InlineData(500)]
    // Comprueba que un precio mayor que cero SÍ sea válido.
    public void PrecioMayorQueCero_EsValido(int precio)
    {
        var libro = CrearLibro();
        libro.Precio = precio;

        var errores = Validate(libro);

        // Comprueba que no haya errores asociados al precio.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Precio)));
    }

    [Theory]
    [InlineData(1, false)]    
    [InlineData(150, false)]   
    [InlineData(151, true)]  
    // Comprueba el límite de longitud del título con varios casos a la vez.
    public void ValidacionLongitudTitulo(int longitud, bool debeTenerError)
    {
        var libro = CrearLibro();
        libro.Titulo = new string('A', longitud);

        var errores = Validate(libro);

        // Determina si hubo un error en la propiedad Titulo.
        bool tieneError = errores.Any(error => error.MemberNames.Contains(nameof(Libro.Titulo)));

        // Comprueba que el resultado coincida con lo esperado para cada caso.
        Assert.Equal(debeTenerError, tieneError);
    }

    // --- Métodos auxiliares (propios de esta clase, no chocan con los otros archivos) ---

    private static IReadOnlyCollection<ValidationResult> Validate(Libro libro)
    {
        var context = new ValidationContext(libro);
        var errores = new List<ValidationResult>();
        Validator.TryValidateObject(libro, context, errores, validateAllProperties: true);
        return errores;
    }

    private static Libro CrearLibro()
    {
        return new Libro
        {
            Titulo = "Cien años de soledad",
            Autor = "Gabriel García Márquez",
            Categoria = "Novela",
            Precio = 15,
            Disponible = true
        };
    }
}