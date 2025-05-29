namespace kutuphane.Models
{
    
    public class SavedBook
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public UserModel User { get; set; }

    public int BookId { get; set; }
    public BookModel Book { get; set; }
}
}