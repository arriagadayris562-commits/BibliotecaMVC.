using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Tests;

// Agrupa las pruebas relacionadas con las reglas de validación de Libro.
public class LibroValidationTests
{
    [Fact]
    // Comprueba que el título del libro sea obligatorio.
    public void TituloEsObligatorio()
    {
        // Crea un libro con el título vacío.
        var libro = CrearLibro();
        libro.Titulo = string.Empty;

        // Ejecuta las validaciones definidas mediante Data Annotations.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a la propiedad Titulo.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Fact]
    // Comprueba que el título no supere los 150 caracteres.
    public void TituloNoPuedeSuperarLos150Caracteres()
    {
        // Crea un título con 151 caracteres.
        var libro = CrearLibro();
        libro.Titulo = new string('A', 151);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda a la propiedad Titulo.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Fact]
    // Comprueba que el límite exacto de 150 caracteres sea válido.
    public void TituloCon150CaracteresEsValido()
    {
        // Crea un título con exactamente 150 caracteres.
        var libro = CrearLibro();
        libro.Titulo = new string('A', 150);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya errores para la propiedad Titulo.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Theory]
    // Cada [InlineData] es un caso de prueba distinto que comparte el mismo método.
    [InlineData(1, false)]     // 1 carácter    -> válido (sin error)
    [InlineData(150, false)]   // límite exacto -> válido
    [InlineData(151, true)]    // excede límite -> error
    // Comprueba el límite de longitud del título con varios casos a la vez.
    public void ValidacionLongitudTitulo(int longitud, bool debeTenerError)
    {
        // Crea un título con la longitud indicada.
        var libro = CrearLibro();
        libro.Titulo = new string('A', longitud);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Determina si hubo un error en la propiedad Titulo.
        bool tieneError = errores.Any(error => error.MemberNames.Contains(nameof(Libro.Titulo)));

        // Comprueba que el resultado coincida con lo esperado para cada caso.
        Assert.Equal(debeTenerError, tieneError);
    }

    [Fact]
    // Comprueba que el autor sea obligatorio.
    public void AutorEsObligatorio()
    {
        // Crea un libro sin autor.
        var libro = CrearLibro();
        libro.Autor = string.Empty;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a la propiedad Autor.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Autor)));
    }

    [Fact]
    // Comprueba que el autor no supere los 100 caracteres.
    public void AutorNoPuedeSuperarLos100Caracteres()
    {
        // Crea un autor con 101 caracteres.
        var libro = CrearLibro();
        libro.Autor = new string('B', 101);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda a la propiedad Autor.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Autor)));
    }

    [Fact]
    // Comprueba que la categoría no supere los 50 caracteres.
    public void CategoriaNoPuedeSuperarLos50Caracteres()
    {
        // Crea un libro con una categoría de 51 caracteres.
        var libro = CrearLibro();
        libro.Categoria = new string('C', 51);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda a la propiedad Categoria.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Categoria)));
    }

    [Theory]
    // Varios precios inválidos (menores o iguales a cero) en una sola prueba.
    [InlineData(0)]     // precio cero     -> inválido
    [InlineData(-1)]    // precio negativo -> inválido
    [InlineData(-10)]   // precio negativo -> inválido
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
    // Varios precios válidos (mayores que cero) en una sola prueba.
    [InlineData(1)]     // precio mínimo válido
    [InlineData(15)]
    [InlineData(500)]
    // Comprueba que un precio mayor que cero SÍ sea válido.
    public void PrecioMayorQueCero_EsValido(int precio)
    {
        // Crea un libro válido y le asigna el precio bajo prueba.
        var libro = CrearLibro();
        libro.Precio = precio;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya errores asociados al precio.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Precio)));
    }

    [Fact]
    // Comprueba que el libro acepte títulos y autores con tildes.
    public void LibroConCaracteresEspecialesEsValido()
    {
        // Crea un libro con nombres que contienen tildes.
        var libro = new Libro
        {
            Titulo = "Cien años de soledad",
            Autor = "Gabriel García Márquez",
            Categoria = "Novela",
            Precio = 15,
            Disponible = true
        };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el libro no tenga errores de validación.
        Assert.Empty(errores);
    }

    // Método auxiliar que concentra la ejecución de las validaciones del modelo.
    private static IReadOnlyCollection<ValidationResult> Validate(Libro libro)
    {
        // Crea el contexto de validación para la instancia recibida.
        var context = new ValidationContext(libro);
        // Prepara la colección donde se almacenarán los errores encontrados.
        var errores = new List<ValidationResult>();
        // Ejecuta todas las validaciones de las propiedades del modelo.
        Validator.TryValidateObject(libro, context, errores, validateAllProperties: true);
        // Devuelve los errores para que cada prueba pueda analizarlos.
        return errores;
    }

    // Crea un libro con datos válidos para utilizarlo en las pruebas.
    private static Libro CrearLibro()
    {
        // Devuelve un libro con todas sus propiedades asignadas.
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