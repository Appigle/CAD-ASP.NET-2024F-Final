using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalTest24F.Models
{
  public class Book
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "ISBN is required")]
    [Display(Name = "ISBN-13")]
    [RegularExpression(@"^(978|979|989)(-?\d{1,5}){4}$",
        ErrorMessage = "ISBN must start with 978, 979, or 989 and follow the ISBN-13 format (e.g., 978-1-86197-876-9)")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Title")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    [Display(Name = "Author")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Author name must be between 1 and 100 characters")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Count is required")]
    [Display(Name = "Number of Copies")]
    [Range(1, int.MaxValue, ErrorMessage = "Count must be at least 1")]
    public int Count { get; set; }
    public ICollection<BookLoan>? BookLoans { get; set; } = new List<BookLoan>();
  }
}