using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carsses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carss = await _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana)
                .FirstOrDefaultAsync(m => m.id == id);

            if (carss == null)
            {
                return NotFound();
            }

            return View(carss);
        }

        // GET: Carsses/Create
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

        // POST: Carsses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("id,model,id_marki,id_str,god_poiz,id_cvet,id_salona,id_kompl,image,price")] Carss carss)
        {
            if (ModelState.IsValid)
            {
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

        // GET: Carsses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carss = await _context.Carss.FindAsync(id);
            if (carss == null)
            {
                return NotFound();
            }
            ViewData["id_kompl"] = new SelectList(_context.compl, "id", "name", carss.id_kompl);
            ViewData["id_cvet"] = new SelectList(_context.cveta, "id", "name", carss.id_cvet);
            ViewData["id_marki"] = new SelectList(_context.Marks, "id", "name", carss.id_marki);
            ViewData["id_salona"] = new SelectList(_context.salonch, "id", "name", carss.id_salona);
            ViewData["id_str"] = new SelectList(_context.strana, "id", "name", carss.id_str);
            return View(carss);
        }

        // POST: Carsses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("id,model,id_marki,id_str,god_poiz,id_cvet,id_salona,id_kompl,image,price")] Carss carss)
        {
            if (id != carss.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carss);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarssExists(carss.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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

        // GET: Carsses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carss = await _context.Carss
                .Include(c => c.compl)
                .Include(c => c.cveta)
                .Include(c => c.Marks)
                .Include(c => c.salonch)
                .Include(c => c.strana)
                .FirstOrDefaultAsync(m => m.id == id);
            if (carss == null)
            {
                return NotFound();
            }

            return View(carss);
        }

        // POST: Carsses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carss = await _context.Carss.FindAsync(id);
            if (carss != null)
            {
                _context.Carss.Remove(carss);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarssExists(int id)
        {
            return _context.Carss.Any(e => e.id == id);
        }
    }
}