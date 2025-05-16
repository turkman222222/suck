using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class bronsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public bronsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: brons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.bron.Include(b => b.Carss).Include(b => b.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: brons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bron = await _context.bron
                .Include(b => b.Carss)
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (bron == null)
            {
                return NotFound();
            }

            return View(bron);
        }

        // GET: brons/Create
        public IActionResult Create()
        {
            ViewData["id_car"] = new SelectList(_context.Carss, "id", "id");
            ViewData["id_usr"] = new SelectList(_context.user, "id_user", "id_user");
            return View();
        }

        // POST: brons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,id_car,id_usr")] bron bron)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bron);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_car"] = new SelectList(_context.Carss, "id", "id", bron.id_car);
            ViewData["id_usr"] = new SelectList(_context.user, "id_user", "id_user", bron.id_usr);
            return View(bron);
        }

        // GET: brons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bron = await _context.bron.FindAsync(id);
            if (bron == null)
            {
                return NotFound();
            }
            ViewData["id_car"] = new SelectList(_context.Carss, "id", "id", bron.id_car);
            ViewData["id_usr"] = new SelectList(_context.user, "id_user", "id_user", bron.id_usr);
            return View(bron);
        }

        // POST: brons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,id_car,id_usr")] bron bron)
        {
            if (id != bron.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bron);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!bronExists(bron.id))
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
            ViewData["id_car"] = new SelectList(_context.Carss, "id", "id", bron.id_car);
            ViewData["id_usr"] = new SelectList(_context.user, "id_user", "id_user", bron.id_usr);
            return View(bron);
        }

        // GET: brons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bron = await _context.bron
                .Include(b => b.Carss)
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (bron == null)
            {
                return NotFound();
            }

            return View(bron);
        }

        // POST: brons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bron = await _context.bron.FindAsync(id);
            if (bron != null)
            {
                _context.bron.Remove(bron);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool bronExists(int id)
        {
            return _context.bron.Any(e => e.id == id);
        }
    }
}
