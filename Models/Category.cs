using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Models
{
    public class Category
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên là danh mục bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên danh mục không vượt quá 50 ký tự")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
