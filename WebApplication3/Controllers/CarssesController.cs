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
using Microsoft.IdentityModel.Tokens;

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

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string filterBrand,
                                              string minPrice, string maxPrice, string sortOrder)
        {
            IQueryable<Carss> carsQuery = _context.Carss.Include(c => c.Marks);

            if (!string.IsNullOrEmpty(searchString))
            {
                carsQuery = carsQuery.Where(c => c.model.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(filterBrand))
            {
                carsQuery = carsQuery.Where(c => c.Marks.name_marka == filterBrand);
            }

            if (!string.IsNullOrEmpty(minPrice) && decimal.TryParse(minPrice, out decimal minPriceValue))
            {
                carsQuery = carsQuery.Where(c => c.price >= minPriceValue);
            }

            if (!string.IsNullOrEmpty(maxPrice) && decimal.TryParse(maxPrice, out decimal maxPriceValue))
            {
                carsQuery = carsQuery.Where(c => c.price <= maxPriceValue);
            }

            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            carsQuery = sortOrder switch
            {
                "price_desc" => carsQuery.OrderByDescending(c => c.price),
                "price_asc" => carsQuery.OrderBy(c => c.price),
                _ => carsQuery.OrderBy(c => c.id)
            };

            ViewBag.Brands = await _context.Marks.Select(m => new { m.name_marka })
                                                .Distinct()
                                                .ToListAsync();

            ViewData["SearchString"] = searchString;
            ViewData["BrandFilter"] = filterBrand;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SortOrder"] = sortOrder;

            return View(await carsQuery.ToListAsync());
        }
        [AllowAnonymous]
        public IActionResult GetImage(int id)
        {
            var car = _context.Carss.FirstOrDefault(c => c.id == id);
            if (car?.image != null)
            {
                return File(car.image, "image/jpeg"); // или другой соответствующий MIME-тип
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int carId)
        {
            try
            {
                var userId = GetCurrentUserId();
                Console.WriteLine($"UserId: {userId}, CarId: {carId}");

                var favorite = await _context.izbr
                    .FirstOrDefaultAsync(i => i.user_id == userId && i.car_id == carId);

                if (favorite == null)
                {
                    _context.izbr.Add(new izbr { user_id = userId, car_id = carId });
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, isFavorite = true, message = "Автомобиль добавлен в избранное" });
                }
                else
                {
                    _context.izbr.Remove(favorite);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, isFavorite = false, message = "Автомобиль удален из избранного" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Произошла ошибка: " + ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> IsFavorite(int carId)
        {
            var userId = GetCurrentUserId();
            var isFavorite = await _context.izbr
                .AnyAsync(i => i.user_id == userId && i.car_id == carId);

            return Json(new { isFavorite });
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            ViewBag.id_marki = new SelectList(_context.Marks, "id", "name_marka");
            ViewBag.id_str = new SelectList(_context.strana, "id", "strana_name");
            ViewBag.id_cvet = new SelectList(_context.cveta, "id", "cvet_name");
            ViewBag.id_salona = new SelectList(_context.salonch, "id", "salon");
            ViewBag.id_kompl = new SelectList(_context.compl, "id", "kompl_name");
            return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("id,model,id_marki,id_str,god_poiz,id_cvet,id_salona,id_kompl,price")] Carss carss, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
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

            if (!ModelState.IsValid)
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
            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "id", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "id", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "id", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "id", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "id", carss.id_str);
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
        
      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var carss = await _context.Carss.FindAsync(id);
                if (carss != null)
                {
                    _context.Carss.Remove(carss);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
      
    }
}

