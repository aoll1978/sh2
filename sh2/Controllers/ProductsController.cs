using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sh2.Data;
using sh2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sh2.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment; // Отвечает за работу с файлами (картинками), на сервере независимый маршрут

        // 2. Обновляем конструктор, добавляя environment
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        //public ProductsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: Products

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products
                .Include(p => p.ProductImages) // Загружаем картинки / Load images
                .Include(p => p.ProductCategories) // Загружаем связи с категориями / Load category relations 
                .ThenInclude(pc => pc.Category); // Загружаем сами категории / Load categories themselves

            return View(await applicationDbContext.ToListAsync());
        }

        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Products.ToListAsync());
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile[] uploadedImages, int[] selectedCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Обработка категорий
                if (selectedCategories != null)
                {
                    foreach (var categoryId in selectedCategories)
                    {
                        _context.ProductCategories.Add(new ProductCategory { ProductId = product.Id, CategoryId = categoryId });
                    }
                }

                // Обработка картинок
                if (uploadedImages != null)
                {
                    foreach (var file in uploadedImages)
                    {
                        if (file.Length > 0)
                        {
                            // 1. Получаем путь к папке wwwroot/images
                            // Get path to wwwroot/images
                            string wwwRootPath = _appEnvironment.WebRootPath;
                            string folderPath = Path.Combine(wwwRootPath, "images");

                            // 2. ПРОВЕРКА: Если папки нет - создаем её! (Fix for DirectoryNotFoundException)
                            // CHECK: If folder doesn't exist - create it!
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            // 3. Формируем уникальное имя файла
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                            string fullPath = Path.Combine(folderPath, fileName);

                            // 4. Сохраняем файл
                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            // 5. Записываем путь в БД (используем ImageUrl, как у вас в модели)
                            var image = new ProductImage
                            {
                                ImageUrl = "/images/" + fileName, // <-- Исправил Url на ImageUrl
                                ProductId = product.Id
                            };
                            _context.ProductImages.Add(image);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        //public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,InStock,Rating,Material,InsertMaterial")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Загружаем товар со всеми связями
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            // Передаем список всех категорий
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Аргументы: id, сам товар, новые картинки, новые категории, ID картинок для удаления
        public async Task<IActionResult> Edit(int id, Product product, IFormFile[] uploadedImages, int[] selectedCategories, int[] imagesToDelete)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Обновляем базовые поля товара (Имя, Цена...)
                    _context.Update(product);

                    // 2. ОБНОВЛЕНИЕ КАТЕГОРИЙ
                    // Сначала удаляем старые связи
                    var existingCategories = _context.ProductCategories.Where(pc => pc.ProductId == id);
                    _context.ProductCategories.RemoveRange(existingCategories);

                    // Потом добавляем новые (если выбраны)
                    if (selectedCategories != null)
                    {
                        foreach (var catId in selectedCategories)
                        {
                            _context.ProductCategories.Add(new ProductCategory { ProductId = id, CategoryId = catId });
                        }
                    }

                    // 3. УДАЛЕНИЕ ВЫБРАННЫХ КАРТИНОК
                    if (imagesToDelete != null && imagesToDelete.Length > 0)
                    {
                        foreach (var imgId in imagesToDelete)
                        {
                            var image = await _context.ProductImages.FindAsync(imgId);
                            if (image != null)
                            {
                                // Удаляем файл с диска
                                var path = Path.Combine(_appEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                                // Удаляем запись из БД
                                _context.ProductImages.Remove(image);
                            }
                        }
                    }

                    // 4. ДОБАВЛЕНИЕ НОВЫХ КАРТИНОК (Копипаст из Create)
                    if (uploadedImages != null)
                    {
                        foreach (var file in uploadedImages)
                        {
                            if (file.Length > 0)
                            {
                                string folderPath = Path.Combine(_appEnvironment.WebRootPath, "images");
                                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                                string fullPath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                _context.ProductImages.Add(new ProductImage { ProductId = id, ImageUrl = "/images/" + fileName });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Если ошибка, возвращаем данные назад
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Загружаем товар ВМЕСТЕ с картинками, чтобы знать пути к файлам
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product != null)
            {
                // 2. Удаляем файлы с диска
                if (product.ProductImages != null)
                {
                    foreach (var image in product.ProductImages)
                    {
                        // image.ImageUrl хранит путь вида "/images/foto.jpg"
                        // Нам нужно превратить его в полный путь "C:\Users\...\wwwroot\images\foto.jpg"
                        var relativePath = image.ImageUrl.TrimStart('/');
                        var absolutePath = Path.Combine(_appEnvironment.WebRootPath, relativePath);

                        if (System.IO.File.Exists(absolutePath))
                        {
                            System.IO.File.Delete(absolutePath);
                        }
                    }
                }

                // 3. Удаляем запись из базы данных (EF Core сам удалит каскадно связи в ProductImages и ProductCategories)
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
