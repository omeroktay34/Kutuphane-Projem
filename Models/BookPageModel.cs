using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kutuphane.Models
{

  public class BookPageModel
  {
    public int Id { get; set; }
    public int BookModelId { get; set; }
    public BookModel? BookModel { get; set; }
    public int PageNumber { get; set; }
    public string Content { get; set; }

}









}