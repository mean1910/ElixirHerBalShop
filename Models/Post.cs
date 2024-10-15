using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề bài viết là bắt buộc")]
        public string Title { get; set; }
        public string ShortContent { get; set; } // Nội dung ngắn
        public string? Content { get; set; }
        public string? PostImage {  get; set; }
        

        public string Author { get; set; } // Tác giả

        public DateTime DateCreate { get; set; } // Ngày tạo

        public bool Status { get; set; } // Trạng thái

    }
}
