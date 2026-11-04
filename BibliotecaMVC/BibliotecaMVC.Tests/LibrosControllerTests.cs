using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

// Agrupa las pruebas relacionadas con las acciones de LibrosController.
public class LibrosControllerTests
{
    [Fact]
    // Comprueba que Index devuelva todos los libros registrados.
    public void Index_DevuelveLaListaDeLibros()
    {
        // Crea una lista de libros para la prueba.
        var libros = new List<Libro>
        {
            new Libro { ID = 1, Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", Precio = 15, Disponible = true },
            new Libro { ID = 2, Titulo = "Don Quijote", Autor = "Miguel de Cervantes", Categoria = "Clásico", Precio = 20, Disponible = true }
        };

        // Crea un repositorio simulado que devuelve los libros registrados.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerTodos()).Returns(libros);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta la acción Index.
        var result = controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Obtiene el modelo de la vista como una colección de libros.
        var resultado = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        // Comprueba que la vista contenga los dos libros.
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    // Comprueba que Details devuelva un libro existente.
    public void Details_ConLibroExistente_DevuelveElLibro()
    {
        // Crea un libro de prueba.
        var libro = CrearLibro();

        // Configura el repositorio para devolver el libro solicitado.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(1)).Returns(libro);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Details con el identificador del libro.
        var result = controller.Details(1);

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista reciba el libro esperado.
        Assert.Same(libro, view.Model);
    }

    [Fact]
    // Comprueba que Details responda NotFound cuando el libro no existe.
    public void Details_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un repositorio que no encuentra ningún libro.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(999)).Returns((Libro)null!);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Details con un identificador inexistente.
        var result = controller.Details(999);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Create guarde un libro válido y redirija al listado.
    public void Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
    {
        // Crea el repositorio simulado.
        var repositorio = new Mock<IRepositorioLibro>();

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Construye el libro que se enviará al controlador.
        var libro = CrearLibro();

        // Ejecuta la acción Create.
        var result = controller.Create(libro);

        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Comprueba que se haya llamado al método para agregar el libro.
        repositorio.Verify(r => r.Agregar(libro), Times.Once);
    }

    [Fact]
    // Comprueba que Create no guarde datos cuando el modelo es inválido.
    public void Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        // Crea el repositorio simulado.
        var repositorio = new Mock<IRepositorioLibro>();

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Simula un error de validación en el título.
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");

        // Crea el libro que se enviará con el modelo inválido.
        var libro = CrearLibro();

        // Ejecuta la acción Create.
        var result = controller.Create(libro);

        // Comprueba que se devuelva nuevamente la vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista conserve el libro enviado.
        Assert.Same(libro, view.Model);
        // Comprueba que no se haya agregado ningún libro.
        repositorio.Verify(r => r.Agregar(It.IsAny<Libro>()), Times.Never);
    }

    [Fact]
    // Comprueba que Edit devuelva un libro existente para mostrarlo.
    public void Edit_ConLibroExistente_DevuelveElLibro()
    {
        // Crea un libro de prueba.
        var libro = CrearLibro();

        // Configura el repositorio para devolver el libro.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(1)).Returns(libro);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Edit con el identificador del libro.
        var result = controller.Edit(1);

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista reciba el libro esperado.
        Assert.Same(libro, view.Model);
    }

    [Fact]
    // Comprueba que Edit devuelva NotFound si el libro no existe.
    public void Edit_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un repositorio que no encuentra el libro.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(999)).Returns((Libro)null!);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Edit con un identificador inexistente.
        var result = controller.Edit(999);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Edit actualice los datos de un libro existente.
    public void Edit_ConLibroValido_ActualizaElLibro()
    {
        // Crea el libro que ya existe en el repositorio.
        var libroExistente = CrearLibro();

        // Configura el repositorio simulado.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(1)).Returns(libroExistente);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Construye el libro con los datos actualizados.
        var libroEditado = new Libro
        {
            ID = 1,
            Titulo = "Cien años de soledad - Edición nueva",
            Autor = "Gabriel García Márquez",
            Categoria = "Novela",
            Precio = 25,
            Disponible = true
        };

        // Ejecuta la acción Edit.
        var result = controller.Edit(1, libroEditado);

        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Comprueba que se haya llamado al método para actualizar.
        repositorio.Verify(r => r.Actualizar(libroEditado), Times.Once);
    }

    [Fact]
    // Comprueba que Edit rechace un identificador diferente al del modelo.
    public void Edit_ConIdentificadorDiferente_DevuelveBadRequest()
    {
        // Crea el repositorio simulado.
        var repositorio = new Mock<IRepositorioLibro>();

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Crea un libro cuyo identificador no coincide con el parámetro.
        var libro = CrearLibro();

        // Ejecuta Edit con dos identificadores diferentes.
        var result = controller.Edit(2, libro);

        // Comprueba que la respuesta sea BadRequest.
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    // Comprueba que Edit no actualice el libro cuando el modelo es inválido.
    public void Edit_ConModeloInvalido_NoActualizaYDevuelveLaVista()
    {
        // Crea el libro existente.
        var libro = CrearLibro();

        // Configura el repositorio simulado.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(1)).Returns(libro);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Simula un error de validación del título.
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");

        // Ejecuta Edit con el modelo inválido.
        var result = controller.Edit(1, libro);

        // Comprueba que se devuelva nuevamente la vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista conserve el libro enviado.
        Assert.Same(libro, view.Model);
        // Comprueba que no se haya actualizado ningún libro.
        repositorio.Verify(r => r.Actualizar(It.IsAny<Libro>()), Times.Never);
    }

    [Fact]
    // Comprueba que Delete elimine un libro existente y redirija al listado.
    public void Delete_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
    {
        // Crea un libro de prueba.
        var libro = CrearLibro();

        // Configura el repositorio para encontrar el libro.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(1)).Returns(libro);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Delete con el identificador del libro.
        var result = controller.Delete(1);

        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Comprueba que se haya llamado al método para eliminar.
        repositorio.Verify(r => r.Eliminar(1), Times.Once);
    }

    [Fact]
    // Comprueba que Delete devuelva NotFound si el libro no existe.
    public void Delete_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un repositorio que no encuentra el libro.
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerPorId(999)).Returns((Libro)null!);

        // Crea el controlador de prueba.
        var controller = CreateController(repositorio);

        // Ejecuta Delete con un identificador inexistente.
        var result = controller.Delete(999);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
        // Comprueba que no se haya llamado al método para eliminar.
        repositorio.Verify(r => r.Eliminar(It.IsAny<int>()), Times.Never);
    }

    // Crea un controlador usando el repositorio simulado y un logger.
    private static LibrosController CreateController(Mock<IRepositorioLibro> repositorio)
    {
        // Crea un mock para no depender de un sistema de logs real.
        var logger = new Mock<ILogger<LibrosController>>();
        // Devuelve una instancia del controlador preparada para la prueba.
        return new LibrosController(repositorio.Object, logger.Object);
    }

    // Crea un libro con datos de prueba.
    private static Libro CrearLibro()
    {
        // Devuelve un libro con sus propiedades asignadas.
        return new Libro
        {
            ID = 1,
            Titulo = "Cien años de soledad",
            Autor = "Gabriel García Márquez",
            Categoria = "Novela",
            Precio = 15,
            Disponible = true
        };

    }
}
