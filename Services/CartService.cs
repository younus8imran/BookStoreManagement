using BookStoreManagement.Models;
using Newtonsoft.Json;

namespace BookStoreManagement.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private List<CartItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartSessionKey);
            return cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(x => x.BookId == item.BookId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            SaveCart(cart);
        }

        public void RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.BookId == bookId);
            SaveCart(cart);
        }

        public List<CartItem> GetCartItems() => GetCart();

        public decimal GetTotalPrice() => GetCart().Sum(x => x.Price * x.Quantity);

        public void ClearCart() => SaveCart(new List<CartItem>());
    }
}
