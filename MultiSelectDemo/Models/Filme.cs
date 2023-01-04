using System.ComponentModel.DataAnnotations;

namespace MultiSelectDemo.Models
{
    public class Filme
    {
        [Key]
        public int FilmeId { get; set; }
        public string Nome { get; set; }
        public virtual ICollection<Categoria>? Categorias { get; set; }
    }
}
