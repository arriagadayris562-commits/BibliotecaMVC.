
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100)]
        public string Autor { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(50)]
        public string Categoria { get; set; }

        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal Precio { get; set; }

        public bool Disponible { get; set; }
    }
}
