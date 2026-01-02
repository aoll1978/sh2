using Microsoft.AspNetCore.Mvc;
using sh2.Data;
using sh2.Models;
using sh2.Extensions; // Обязательно подключаем наши расширения

namespace sh2.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Показать корзину
        public IActionResult Index()
        {
            // Достаем корзину из сессии. Если там пусто - создаем новую пустую корзину.
            Cart cart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
            return View(cart);
        }

        // Добавить товар
        public IActionResult AddToCart(int id)
        {
            Product? product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // 1. Получаем текущую корзину
                Cart cart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();

                // 2. Добавляем товар (по 1 штуке за клик)
                cart.AddItem(product, 1);

                // 3. Сохраняем обновленную корзину обратно в сессию
                HttpContext.Session.SetObject("Cart", cart);
            }

            // Перенаправляем пользователя на страницу просмотра корзины
            return RedirectToAction("Index");
        }

        // Удалить товар
        public IActionResult RemoveFromCart(int id)
        {
            Cart cart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();

            cart.RemoveItem(id);

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}