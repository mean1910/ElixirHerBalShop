using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Elixir.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Mã sản phẩm")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 kí tự")]
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }

        [Range(1, 100000000, ErrorMessage = "Giá sản phẩm phải nằm trong khoảng từ 1 đến 100000000")]
        [DisplayName("Giá bán")]
        public decimal Price { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Mô tả ngắn")]
        public string ShortDescription { get; set; } // Mô tả ngắn

        [DisplayName("Hình ảnh")]
        public string? ImageUrl { get; set; }

        public List<ProductImage>? Images { get; set; }

        [DisplayName("Mã danh mục")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Danh mục")]
        public Category? Category { get; set; }

        [DisplayName("Số lượng hàng trong kho")]
        public int QuantityInStock { get; set; } // Số lượng hàng trong kho

        [DisplayName("Đơn vị tính")]
        public int unit {  get; set; }

        [DisplayName("Trạng thái")]
        public bool Status { get; set; } // Trạng thái: Bày bán hoặc Gỡ xuống

        [DisplayName("Mã chương trình giảm giá")]
        [ForeignKey("Discount")]
        public int? DiscountId { get; set; }  // Nullable

        [DisplayName("Giảm giá theo phần trăm")]
        public Discount? Discounts { get; set; }

        [DisplayName("Trang chủ")]
        public bool HomeFlag { get; set; } // HomeFlag: Đánh dấu sản phẩm được gắn lên trang chủ
    }


}
