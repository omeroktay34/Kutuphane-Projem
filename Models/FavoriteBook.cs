namespace kutuphane.Models
{
    public class FavoriteBook
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }

        public UserModel User { get; set; }
        public BookModel Book { get; set; }  
    
}

}