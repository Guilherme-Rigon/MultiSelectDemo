using System.ComponentModel.DataAnnotations;

namespace MultiSelectDemo.Models
{
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public virtual ICollection<Filme>? Filmes { get; set; }
    }
}
