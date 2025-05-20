using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class izbrsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public izbrsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            var favorites = await _context.izbr
                .Where(i => i.user_id == userId)
                .Include(i => i.Carss)
                    .ThenInclude(c => c.Marks)
                .Include(i => i.Carss)
                    .ThenInclude(c => c.cveta)
                .ToListAsync();

            return View(favorites);
        }

        // POST: Favorites/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int carId)
        {
            var userId = GetCurrentUserId();

            if (!await _context.Carss.AnyAsync(c => c.id == carId))
                return NotFound();

            var exists = await _context.izbr
                .AnyAsync(i => i.user_id == userId && i.car_id == carId);

            if (!exists)
            {
                _context.izbr.Add(new izbr
                {
                    user_id = userId,
                    car_id = carId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Favorites/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetCurrentUserId();
            var favorite = await _context.izbr
                .FirstOrDefaultAsync(i => i.id == id && i.user_id == userId);

            if (favorite == null)
                return NotFound();

            _context.izbr.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}