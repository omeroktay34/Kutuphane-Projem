using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using kutuphane.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;



namespace kutuphane.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public HomeController(AppDbContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MainMenu(string? search = null)
    {

var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}



        CategoryModel cat = new() { Name = "test", };
        var latestBooks = _context.Kitaplar.ToList();

        ViewBag.LatestBooks = latestBooks;
        ViewBag.LatestBooks = latestBooks;
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login");

        var user = await _context.Kullanicilar.FindAsync(userId);

        var mainCategories = await _context.Kategoriler
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .ToListAsync();

        ViewBag.MainCategories = mainCategories;

        List<BookModel> searchResults = new List<BookModel>();
        if (!string.IsNullOrEmpty(search))
        {
            searchResults = await _context.Kitaplar
                .Where(b => b.Title.Contains(search))
                .ToListAsync();
        }

        ViewBag.SearchResults = searchResults;
        ViewBag.SearchQuery = search;

        var lastRead = await _context.UserLastReadPages
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        BookModel? lastReadBook = null;
        int? lastReadPageNumber = null;

        if (lastRead != null)
        {
            lastReadBook = await _context.Kitaplar.FindAsync(lastRead.BookId);
            lastReadPageNumber = lastRead.PageNumber;
        }

        ViewBag.LastReadBook = lastReadBook;
        ViewBag.LastReadPageNumber = lastReadPageNumber;

        return View(user);
    }











    public IActionResult Library(int? categoryId)
    {

var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}



        IQueryable<BookModel> kitaplar = _context.Kitaplar.Include(b => b.Category);

        if (categoryId.HasValue)
        {
            var selectedCategory = _context.Kategoriler
                .FirstOrDefault(c => c.Id == categoryId.Value);

            if (selectedCategory != null)
            {
                if (selectedCategory.ParentCategoryId == null)
                {

                    var altKategoriIdler = _context.Kategoriler
                        .Where(c => c.ParentCategoryId == selectedCategory.Id)
                        .Select(c => c.Id)
                        .ToList();

                    kitaplar = kitaplar.Where(k => altKategoriIdler.Contains(k.CategoryId));
                }
                else
                {

                    kitaplar = kitaplar.Where(k => k.CategoryId == categoryId.Value);
                }
            }
        }

        return View(kitaplar.ToList());
    }















    public IActionResult GetBookCover(int bookId)
    {
        var book = _context.Kitaplar.Find(bookId);

        if (book == null || book.CoverImage == null)
        {
            return File("~/images/default_cover.png", "image/png");
        }

        return File(book.CoverImage, "image/jpeg");
    }

















    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

[HttpPost]
public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
{
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        ViewBag.Error = "Kullanıcı adı ve şifre boş olamaz.";
        return View();
    }

    var user = await _context.Kullanicilar
        .FirstOrDefaultAsync(u => u.Username == username);

    if (user == null)
    {
        ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
        return View();
    }

    var hashedInputPassword = HashPassword(password);

    if (user.Password != hashedInputPassword)
    {
        ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
        return View();
    }


    HttpContext.Session.SetInt32("UserId", user.Id);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = rememberMe,
        ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    if (rememberMe)
    {
        Response.Cookies.Append("RememberedUsername", username, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(30),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }
    else
    {
        Response.Cookies.Delete("RememberedUsername");
    }

    return RedirectToAction("MainMenu");
}






    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Tüm alanlar zorunludur.");
            return View();
        }

        var userExists = await _context.Kullanicilar.AnyAsync(u => u.Email == email);
        if (userExists)
        {
            ModelState.AddModelError("", "Bu email zaten kayıtlı.");
            return View();
        }

        var user = new UserModel
        {
            Username = username,
            Email = email,
            Password = password
        };

        _context.Kullanicilar.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
        return View();
    }










    int aktifKullaniciId = 1;

    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login");

        var user = await _context.Kullanicilar.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login");

        return View(user);
    }



    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login");

        var user = await _context.Kullanicilar.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login");

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(UserModel model, IFormFile? profileImage)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login");

        var user = await _context.Kullanicilar.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login");


        user.FullName = model.FullName;
        user.Bio = model.Bio;

        if (profileImage != null && profileImage.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                await profileImage.CopyToAsync(ms);
                user.ProfileImage = ms.ToArray();
            }
        }

        _context.Kullanicilar.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profile");
    }












    public IActionResult Logout()
    {

        HttpContext.Session.Clear();


        return RedirectToAction("Login");
    }










    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {

var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}




        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction("MainMenu");
        }

        query = query.ToLower();

        var results = await _context.Kitaplar
            .Where(b => b.Title.ToLower().Contains(query) || b.Author.ToLower().Contains(query))
            .ToListAsync();

        ViewBag.SearchQuery = query;
        return View("SearchResults", results);
    }










    [HttpGet]
    public async Task<IActionResult> SearchResults(string search)
    {





var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}




        if (string.IsNullOrWhiteSpace(search))
        {

            return RedirectToAction("MainMenu");
        }

        var results = await _context.Kitaplar
            .Where(b => b.Title.Contains(search))
            .ToListAsync();

        ViewBag.SearchQuery = search;
        return View(results);
    }










    [HttpGet]
    public async Task<IActionResult> EditAccount()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login");

        var user = await _context.Kullanicilar.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login");

        var model = new AccountEditModel
        {
            Username = user.Username,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount(AccountEditModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login");

        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Kullanicilar.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login");


        user.Username = model.Username;
        user.Email = model.Email;

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) || model.CurrentPassword != user.Password)
            {
                ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                return View(model);
            }

            user.Password = model.NewPassword;
        }

        _context.Kullanicilar.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profile");
    }











 public IActionResult AddBook()
 {


var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}



 var mainCategories = _context.Kategoriler
 .Where(c => c.ParentCategoryId == null)
 .Include(c => c.SubCategories)
 .ToList();

 ViewBag.MainCategories = mainCategories;

 return View(mainCategories);
 }

[HttpPost]
public IActionResult AddBook(BookModel book, IFormFile? CoverImageFile)
{




var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}


    var kategori = _context.Kategoriler.FirstOrDefault(c => c.Id == book.CategoryId);
    if (kategori == null)
    {
        ModelState.AddModelError("CategoryId", "Geçersiz kategori seçildi.");
        return View(book);
    }

    if (CoverImageFile != null && CoverImageFile.Length > 0)
    {
        using (var memoryStream = new MemoryStream())
        {
            CoverImageFile.CopyTo(memoryStream);
            book.CoverImage = memoryStream.ToArray();
        }
    }

    book.CreatedByUserId = 1;
    _context.Kitaplar.Add(book);

    try
    {
        _context.SaveChanges();
    }
    catch (Exception ex)
    {
        ViewBag.Hata = ex.Message;
        return View(book);
    }

    return RedirectToAction("Library");
}







    private List<CategoryModel> GetCategories()
    {
        
        return new List<CategoryModel>
    {
        new CategoryModel
        {
            Id = 1,
            Name = "Roman",
            SubCategories = new List<CategoryModel>
            {
                new CategoryModel { Id = 2, Name = "Tarihî Roman" },
                new CategoryModel { Id = 1, Name = "Polisiye Roman" }
            }
        },
        new CategoryModel
        {
            Id = 2,
            Name = "Şiir",
            SubCategories = new List<CategoryModel>
            {
                new CategoryModel { Id = 3, Name = "Modern Şiir" },
                new CategoryModel { Id = 4, Name = "Klasik Şiir" }
            }
        },
        new CategoryModel
        {
            Id = 3,
            Name = "Manga",
            SubCategories = new List<CategoryModel>
            {
                new CategoryModel { Id = 5, Name = "Shonen" },
                new CategoryModel { Id = 6, Name = "Shoujo" }
            }
        }
    };
    }


private void SeedCategories(AppDbContext context)
{
    








    
    if (!context.Kategoriler.Any())
        {
            var categories = new List<CategoryModel>
        {
          
            new CategoryModel { Id = 1, Name = "Roman", ParentCategoryId = null },
            new CategoryModel { Id = 2, Name = "Şiir", ParentCategoryId = null },
            new CategoryModel { Id = 3, Name = "Manga", ParentCategoryId = null },
            
           
            new CategoryModel { Id = 4, Name = "Polisiye Roman", ParentCategoryId = 1 },
            new CategoryModel { Id = 5, Name = "Tarihi Roman", ParentCategoryId = 1 },
            
        
            new CategoryModel { Id = 6, Name = "Modern Şiir", ParentCategoryId = 2 },
            new CategoryModel { Id = 7, Name = "Klasik Şiir", ParentCategoryId = 2 },
            
           
            new CategoryModel { Id = 8, Name = "Shonen", ParentCategoryId = 3 },
            new CategoryModel { Id = 9, Name = "Shoujo", ParentCategoryId = 3 }
        };

            context.Kategoriler.AddRange(categories);
            context.SaveChanges();
        }
}










    public IActionResult BookDetails(int id)
    {
        var book = _context.Kitaplar
            .Include(b => b.Category)
            .Include(b => b.Pages)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
            return NotFound();

        return View(book);
    }



    [HttpGet]
    public IActionResult AddBookPage(int bookId)
    {


        var page = new BookPageModel
        {
            BookModelId = bookId
        };

        return View(page);
    }

    [HttpPost]
    public IActionResult AddBookPage(BookPageModel page)
    {

        Console.WriteLine("Form gönderildi: " + page.PageNumber + " - " + page.Content);

        if (!ModelState.IsValid)
        {
            return View(page);
        }

        if (page.BookModelId == 0)
        {
            ModelState.AddModelError("", "Kitap ID bulunamadı.");
            return View(page);
        }

        _context.BookPages.Add(page);
        _context.SaveChanges();

        return RedirectToAction("BookDetails", new { id = page.BookModelId });
    }






























































    [HttpGet]
    public IActionResult EditBook(int id)
    {
        var book = _context.Kitaplar.FirstOrDefault(b => b.Id == id);
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> EditBook(BookModel model, IFormFile coverImage)
    {
        var existingBook = _context.Kitaplar.FirstOrDefault(b => b.Id == model.Id);
        if (existingBook != null)
        {
            existingBook.Title = model.Title;
            existingBook.Author = model.Author;
            existingBook.PageCount = model.PageCount;
            existingBook.CategoryId = model.CategoryId;

            if (coverImage != null)
            {
                using var ms = new MemoryStream();
                await coverImage.CopyToAsync(ms);
                existingBook.CoverImage = ms.ToArray();
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("MainMenu");
    }











 public IActionResult ReadBook(int bookId, int startPage = 1, bool showAll = false)
{
    var book = _context.Kitaplar
        .Include(b => b.Pages.OrderBy(p => p.PageNumber))
        .FirstOrDefault(b => b.Id == bookId);

    if (book == null)
        return NotFound();

    var pages = book.Pages.ToList();

    ViewBag.BookId = bookId;
    ViewBag.CurrentStartPage = startPage;
    ViewBag.TotalPages = pages.Count;
    ViewBag.ShowAll = showAll;

    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId != null)
    {
        var existing = _context.UserLastReadPages
            .FirstOrDefault(x => x.UserId == userId);

      
        bool shouldUpdate = false;
        
        if (existing == null)
        {
            _context.UserLastReadPages.Add(new UserBookProgressModel
            {
                UserId = userId.Value,
                BookId = bookId,
                PageNumber = startPage
            });
            shouldUpdate = true;
        }
        else if (existing.BookId != bookId || existing.PageNumber != startPage)
        {
            existing.BookId = bookId;
            existing.PageNumber = startPage;
            _context.UserLastReadPages.Update(existing);
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            _context.SaveChanges();
        }
    }

    return View(pages);
}





    public IActionResult ResumeReading()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        var lastRead = _context.UserLastReadPages
            .FirstOrDefault(x => x.UserId == userId);

        if (lastRead == null)
        {

            TempData["Message"] = "Henüz bir kitap okumaya başlamadınız.";
            return RedirectToAction("MainMenu");
        }



        return RedirectToAction("ReadBook", new { bookId = lastRead.BookId, startPage = lastRead.PageNumber });
    }













public IActionResult Saved()
{



var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}


    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null)
        return RedirectToAction("Login");

    var savedBooks = _context.SavedBooks
        .Include(sb => sb.Book) 
        .Where(sb => sb.UserId == userId)
        .Select(sb => sb.Book)
        .ToList();

    return View(savedBooks);
}












[HttpPost]
public IActionResult SaveBook(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login");

  
    var alreadySaved = _context.SavedBooks.Any(sb => sb.BookId == bookId && sb.UserId == userId.Value);
    if (!alreadySaved)
    {
        var savedBook = new SavedBook
        {
            UserId = userId.Value,
            BookId = bookId
        };

        _context.SavedBooks.Add(savedBook);
        _context.SaveChanges();
    }

    return RedirectToAction("Saved");
}


[HttpPost]
public IActionResult UnsaveBook(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var saved = _context.SavedBooks.FirstOrDefault(s => s.UserId == userId && s.BookId == bookId);
    if (saved != null)
    {
        _context.SavedBooks.Remove(saved);
        _context.SaveChanges();
    }

    return Ok();
}

[HttpPost]
public IActionResult AddFavorite(int bookId)
{

var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}


    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var existing = _context.FavoriteBooks.FirstOrDefault(f => f.UserId == userId && f.BookId == bookId);
    if (existing == null)
    {
        _context.FavoriteBooks.Add(new FavoriteBook { UserId = userId.Value, BookId = bookId });
        _context.SaveChanges();
    }

    
    return RedirectToAction("Favorite");
}


[HttpPost]
public IActionResult RemoveFavorite(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var favorite = _context.FavoriteBooks.FirstOrDefault(f => f.UserId == userId && f.BookId == bookId);
    if (favorite != null)
    {
        _context.FavoriteBooks.Remove(favorite);
        _context.SaveChanges();
    }

    return RedirectToAction("Favorite");
}


[HttpPost]
public IActionResult RemoveSaved(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var saved = _context.SavedBooks.FirstOrDefault(s => s.UserId == userId && s.BookId == bookId);
    if (saved != null)
    {
        _context.SavedBooks.Remove(saved);
        _context.SaveChanges();
    }

    return RedirectToAction("Saved");
}




[HttpPost]
public IActionResult ToggleSaveBook(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var existing = _context.SavedBooks.FirstOrDefault(s => s.UserId == userId && s.BookId == bookId);
    if (existing == null)
    {
        _context.SavedBooks.Add(new SavedBook { UserId = userId.Value, BookId = bookId });
    }
    else
    {
        _context.SavedBooks.Remove(existing);
    }
    _context.SaveChanges();

    return Ok();
}

[HttpPost]
public IActionResult ToggleFavoriteBook(int bookId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return Unauthorized();

    var existing = _context.FavoriteBooks.FirstOrDefault(f => f.UserId == userId && f.BookId == bookId);
    if (existing == null)
    {
        _context.FavoriteBooks.Add(new FavoriteBook { UserId = userId.Value, BookId = bookId });
    }
    else
    {
        _context.FavoriteBooks.Remove(existing);
    }
    _context.SaveChanges();

    return Ok();
}




public IActionResult Favorite()



    {


        var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}

    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login");


    var favoriteBooks = _context.FavoriteBooks
        .Where(f => f.UserId == userId)
        .Include(f => f.Book) 
        .Select(f => f.Book)
        .ToList();

    return View(favoriteBooks); 
}

























[HttpGet]
public IActionResult Settings()
{

var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}



    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login", "Account");

    var user = _context.Kullanicilar.Find(userId);
    return View(user);
}

[HttpPost]
public IActionResult Settings(UserModel model)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null) return RedirectToAction("Login", "Account");

    var user = _context.Kullanicilar.Find(userId);
    if (user == null) return NotFound();

    user.Theme = model.Theme;
    user.Language = model.Language;
user.FontFamily = model.FontFamily;
user.FontSize = model.FontSize;

    _context.SaveChanges();

    return RedirectToAction("Settings");
}







        public override void OnActionExecuting(ActionExecutingContext context)
        {








            
            int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Id == userId);
            ViewBag.Theme = user?.Theme ?? "light";
            ViewBag.Language = user?.Language ?? "tr";
                ViewBag.FontFamily = user?.FontFamily ?? "Arial";
ViewBag.FontSize = user?.FontSize ?? 16;

            }
        else
        {
            ViewBag.Theme = "light";
            ViewBag.Language = "tr";
        }

            base.OnActionExecuting(context);
        }

















public IActionResult PopularBooks()
{


var currentUserId = HttpContext.Session.GetInt32("UserId");
if (!currentUserId.HasValue)
{
    TempData["Error"] = "Lütfen giriş yapın.";
    return RedirectToAction("Login");
}

    var popularBooks = _context.FavoriteBooks
        .Include(f => f.Book)
        .GroupBy(f => f.BookId)
        .Select(g => new PopularBookViewModel
        {
            Book = g.First().Book,
            FavoriteCount = g.Count()
        })
        .OrderByDescending(g => g.FavoriteCount)
        .Take(10)
        .ToList();

    return View(popularBooks);
}






[HttpGet]
public IActionResult ForgotPassword()
{
    return View();
}

[HttpPost]
public IActionResult ForgotPassword(string email)
{
    return RedirectToAction("ResetPassword", new { id = 1 });
}






public IActionResult About()
{





   return View();
}



    [HttpGet]
    public IActionResult ResetPassword(int id)
    {
        var user = _context.Kullanicilar.Find(id);
        if (user == null)
        {
            return NotFound();
        }

 
        return View(user);
    }


    [HttpPost]
    public IActionResult ResetPassword(int id, string newPassword)
    {
        var user = _context.Kullanicilar.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            ViewBag.Error = "Yeni şifre boş olamaz.";
            return View(user);
        }

      
        user.Password = HashPassword(newPassword); 

        _context.SaveChanges();

      
        return RedirectToAction("Login");
    }

    
    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }







    public IActionResult SSS()
    {
        return View();
}



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
