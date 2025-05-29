using Newtonsoft.Json;

namespace kutuphane.Models

{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public int? ParentCategoryId { get; set; }


        [JsonIgnore]
        public CategoryModel? ParentCategory { get; set; }


        public ICollection<CategoryModel> SubCategories { get; set; }
        


        
    }


}