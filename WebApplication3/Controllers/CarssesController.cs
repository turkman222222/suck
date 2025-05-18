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
        public async Task<IActionResult> Index(string searchString, int? markaid)
        {
            var cars = _context.Carss.Include(r => r.Marks).Include(r => r.strana).Include(r => r.cveta).Include(r => r.salonch).Include(r => r.compl).AsQueryable();
            if (markaid.HasValue)
            {
                cars = cars.Where(r => r.id_marki == markaid);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(r => r.model.Contains(searchString));
            }
            return View(await cars.ToListAsync());
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
                _context.izbr.Add(new izbr { car_id = carId, user_id = userId });
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true, message = "Добавлено в избранное" });
            }
            else
            {
                _context.izbr.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false, message = "Удалено из избранного" });
            }
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
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
            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "name");
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "name");
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name");
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "name");
            ViewData["id_str"] = new SelectList(_context.strana, "id", "name");
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

            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "name", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "name", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "name", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "name", carss.id_str);
            return View(carss);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carss = await _context.Carss.FindAsync(id);
            if (carss == null) return NotFound();

            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "name", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "name", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "name", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "name", carss.id_str);
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
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            carss.image = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Сохраняем существующее изображение
                        var existingCar = await _context.Carss.AsNoTracking().FirstOrDefaultAsync(c => c.id == id);
                        if (existingCar != null)
                        {
                            carss.image = existingCar.image;
                        }
                    }

                    _context.Update(carss);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarssExists(carss.id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "name", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "name", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "name", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "name", carss.id_str);
            return View(carss);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carss = await _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana)
                .FirstOrDefaultAsync(m => m.id == id);

            if (carss == null) return NotFound();
            return View(carss);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carss = await _context.Carss.FindAsync(id);
            if (carss != null)
            {
                _context.Carss.Remove(carss);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult GetImage(int id)
        {
            var car = _context.Carss.Find(id);
            if (car?.image != null)
            {
                return File(car.image, "image/jpeg");
            }
            return NotFound();
        }

        private bool CarssExists(int id)
        {
            return _context.Carss.Any(e => e.id == id);
        }
    }
}