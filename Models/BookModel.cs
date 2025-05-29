namespace kutuphane.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int CategoryId { get; set; }
        public CategoryModel? Category { get; set; }
        public int PageCount { get; set; }
        public int CreatedByUserId { get; set; }
        public UserModel? CreatedByUser { get; set; }
        public byte[]? CoverImage { get; set; }
      


    public ICollection<BookPageModel> Pages { get; set; } = new List<BookPageModel>();





    }
}
