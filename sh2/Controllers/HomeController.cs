using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sh2.Data;
using sh2.Models;
using System.Diagnostics;

namespace sh2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Добавляем поле для контекста базы данных
        // Add a field for the database context
        private readonly ApplicationDbContext _context;

        // Внедряем контекст через конструктор
        // Inject the context via the constructor
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Index()
        {
            // Получаем список товаров из базы данных асинхронно
            // Get the list of products from the database asynchronously
            var products = await _context.Products
                .Include(p => p.ProductImages) // Eager Loading  Включаем связанные изображения продуктов / Include related product images
                .AsNoTracking() // Оптимизация для чтения (без отслеживания изменений) / Optimization for reading (no tracking)
                .ToListAsync();

            // Передаем данные (модель) в представление
            // Pass the data (model) to the view
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
