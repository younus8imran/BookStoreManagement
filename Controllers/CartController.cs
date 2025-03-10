using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using BookStoreManagement.Models;
using BookStoreManagement.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize(Roles= "Customer")]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();
        ViewBag.TotalPrice = cartItems.Sum(i => i.Price * i.Quantity);
        return View(cartItems);
    }

    public IActionResult Add(int bookId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var book = _context.Books.Find(bookId);
        if (book != null && userId != null)
        {
            var cartItem = _context.CartItems.FirstOrDefault(c => c.BookId == bookId && c.UserId == userId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    BookId = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Quantity = 1
                });
            }
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Remove(int bookId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartItem = _context.CartItems.FirstOrDefault(c => c.BookId == bookId && c.UserId == userId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartItems = _context.CartItems.Where(c => c.UserId == userId);
        _context.CartItems.RemoveRange(cartItems);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
