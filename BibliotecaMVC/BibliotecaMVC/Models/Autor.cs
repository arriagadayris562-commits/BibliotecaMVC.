using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Autor
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string? Apellido { get; set; } // <--- Ponle el ? para que sea opcional

        [StringLength(50)]
        public string? Nacionalidad { get; set; } // <--- Ponle el ? para que sea opcional

        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        public bool Activo { get; set; } = true;
    }
}