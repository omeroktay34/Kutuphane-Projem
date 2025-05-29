namespace kutuphane.Models
{
    public class UserBookProgressModel
 {
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserModel? User { get; set; }

        public int BookId { get; set; }
        public BookModel? Book { get; set; }

        public int PageNumber { get; set; }
    }

}