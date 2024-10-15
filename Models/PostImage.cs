using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Models
{
    public class PostImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Url { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
