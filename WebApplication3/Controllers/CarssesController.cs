using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.SqlClient;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CarssesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarssesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carsses
        [AllowAnonymous]
     
        public async Task<IActionResult> Index(
    string searchString,
    int? markaid,
    string sortOrder,
    string filterBrand,
    decimal? minPrice,
    decimal? maxPrice)
        {
            // Основной запрос с включением связанных данных
            var carsQuery = _context.Carss
                .Include(c => c.Marks)
                .Include(c => c.strana)
                .Include(c => c.cveta)
                .Include(c => c.salonch)
                .Include(c => c.compl)
                .AsQueryable();

            // Фильтр по поисковой строке (модели)
            if (!string.IsNullOrEmpty(searchString))
            {
                carsQuery = carsQuery.Where(c => c.model.Contains(searchString));
            }

            // Фильтр по марке (ID)
            if (markaid.HasValue)
            {
                carsQuery = carsQuery.Where(c => c.id_marki == markaid.Value);
            }

            // Фильтр по марке (названию)
            if (!string.IsNullOrEmpty(filterBrand))
            {
                carsQuery = carsQuery.Where(c => c.Marks.name_marka == filterBrand);
            }

            // Фильтр по минимальной цене
            if (minPrice.HasValue)
            {
                carsQuery = carsQuery.Where(c => c.price >= minPrice.Value);
            }

            // Фильтр по максимальной цене
            if (maxPrice.HasValue)
            {
                carsQuery = carsQuery.Where(c => c.price <= maxPrice.Value);
            }

            // Сортировка
            switch (sortOrder)
            {
                case "price_desc":
                    carsQuery = carsQuery.OrderByDescending(c => c.price);
                    break;
                case "price_asc":
                    carsQuery = carsQuery.OrderBy(c => c.price);
                    break;
                default:
                    carsQuery = carsQuery.OrderBy(c => c.id);
                    break;
            }

            // Сохраняем параметры фильтрации для представления
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["BrandFilter"] = filterBrand;
            ViewData["SearchString"] = searchString;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            // Получаем список всех марок для dropdown
            ViewBag.Brands = await _context.Marks.ToListAsync();
            ViewBag.MarkaId = new SelectList(_context.Marks, "id", "name_marka", markaid);

            return View(await carsQuery.ToListAsync());
        }

        [HttpGet]
        public IActionResult IsFavorite(int carId)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { isFavorite = false });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isFavorite = _context.izbr.Any(i => i.car_id == carId && i.user_id == userId);

            return Json(new { isFavorite });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int carId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var favorite = await _context.izbr
                .FirstOrDefaultAsync(i => i.car_id == carId && i.user_id == userId);

            if (favorite == null)
            {
                // Добавление в избранное
                _context.izbr.Add(new izbr { car_id = carId, user_id = userId });
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true, message = "Добавлено в избранное" });
            }
            else
            {
                // Удаление из избранного
                _context.izbr.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false, message = "Удалено из избранного" });
            }
        }

        [Authorize]
        public async Task<IActionResult> izbrs()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var favoriteCars = await _context.izbr
                .Where(i => i.user_id == userId)
                .Include(i => i.Carss)
                .ThenInclude(c => c.Marks)
                .Select(i => i.Carss)
                .ToListAsync();

            return View(favoriteCars);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana)
                .FirstOrDefaultAsync(m => m.id == id);

            if (car == null) return NotFound();
            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "kompl_name");
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "cvet_name");
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name_marka");
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "salon");
            ViewData["id_str"] = new SelectList(_context.strana, "id", "strana_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("id,model,id_marki,id_str,god_poiz,id_cvet,id_salona,id_kompl,price")] Carss carss, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(ms);
                        carss.image = ms.ToArray();
                    }
                }

                _context.Add(carss);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "kompl_name");
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "cvet_name");
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name_marka");
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "salon");
            ViewData["id_str"] = new SelectList(_context.strana, "id", "strana_name");
            return View(carss);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carss = await _context.Carss.FindAsync(id);
            if (carss == null) return NotFound();

            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "kompl_name");
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "cvet_name");
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name_marka");
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "salon");
            ViewData["id_str"] = new SelectList(_context.strana, "id", "strana_name");
            return View(carss);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("id,model,id_marki,id_str,god_poiz,id_cvet,id_salona,id_kompl,price")] Carss carss, IFormFile imageFile)
        {
            if (id != carss.id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var carToUpdate = await _context.Carss.FindAsync(id);
                    if (carToUpdate == null) return NotFound();

                    // Обновляем только изменяемые поля
                    carToUpdate.model = carss.model;
                    carToUpdate.id_marki = carss.id_marki;
                    carToUpdate.id_str = carss.id_str;
                    carToUpdate.god_poiz = carss.god_poiz;
                    carToUpdate.id_cvet = carss.id_cvet;
                    carToUpdate.id_salona = carss.id_salona;
                    carToUpdate.id_kompl = carss.id_kompl;
                    carToUpdate.price = carss.price;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            carToUpdate.image = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Сохраняем существующее изображение, если новое не загружено
                        var existingCar = await _context.Carss.AsNoTracking().FirstOrDefaultAsync(c => c.id == id);
                        if (existingCar != null && existingCar.image != null)
                        {
                            carToUpdate.image = existingCar.image;
                        }
                    }

                    _context.Update(carToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    
                    ModelState.AddModelError("", "Не удалось сохранить изменения. Попробуйте еще раз.");
                    _context.Entry(carss).State = EntityState.Detached;
                }
            }

            // Если что-то пошло не так, заново заполняем списки
            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "kompl_name", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "cvet_name", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name_marka", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "salon", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "strana_name", carss.id_str);
            return View(carss);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();

            var car = await _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana)
                .FirstOrDefaultAsync(m => m.id == id);

            if (car == null) return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Не удалось удалить. Попробуйте еще раз, и если проблема сохранится, обратитесь к системному администратору.";
            }

            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Сначала удаляем все записи из избранного для этого автомобиля
                    var favorites = await _context.izbr
                        .Where(i => i.car_id == id)
                        .ToListAsync();

                    _context.izbr.RemoveRange(favorites);
                    await _context.SaveChangesAsync();

                    // Затем удаляем сам автомобиль
                    var car = await _context.Carss.FindAsync(id);
                    if (car != null)
                    {
                        _context.Carss.Remove(car);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
                }
            }
        }
    }
}

