using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sh2.Data;
using sh2.Extensions;
using sh2.Models;

namespace sh2.Controllers
{
    [Authorize] // Требуем вход. Чтобы разрешить гостям - закомментировать эту строку.
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            // 1. Получаем корзину
            var cart = HttpContext.Session.GetObject<Cart>("Cart");

            // Если корзины нет или она пустая - отправляем в каталог
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // 2. Создаем заготовку заказа
            var order = new Order();

            // 3. Если пользователь авторизован, попробуем предзаполнить данные
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    order.Email = user.Email;
                    order.FullName = user.UserName; // Проверить!!!
                    order.UserId = user.Id;
                }
            }

            // Передаем и заказ (для формы), и корзину (для отображения списка товаров справа)
            ViewBag.Cart = cart;
            return View(order);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // 1. Привязываем пользователя (если залогинен)
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    order.UserId = user?.Id;
                }

                // 2. Заполняем детали заказа данными из корзины
                order.OrderDate = DateTime.Now;
                order.Status = OrderStatus.New;
                order.TotalAmount = cart.ComputeTotalValue();

                // 3. Создаем OrderItems (строки заказа)
                foreach (var item in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price = item.Product.Price // Фиксируем цену на момент покупки!
                    };
                    order.OrderItems.Add(orderItem);
                }

                // 4. Сохраняем в БД
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 5. Очищаем корзину
                cart.Clear();
                HttpContext.Session.SetObject("Cart", cart);

                // 6. Переходим на страницу благодарности
                return RedirectToAction("OrderConfirmed", new { id = order.Id });
            }

            // Если ошибка валидации - возвращаем форму обратно
            ViewBag.Cart = cart;
            return View(order);
        }

        public IActionResult OrderConfirmed(int id)
        {
            return View(id);
        }
    }
}