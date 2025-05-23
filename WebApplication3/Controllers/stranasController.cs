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
    public class stranasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public stranasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: stranas
        public async Task<IActionResult> Index()
        {
            return View(await _context.strana.ToListAsync());
        }

        // GET: stranas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var strana = await _context.strana
                .FirstOrDefaultAsync(m => m.id == id);
            if (strana == null)
            {
                return NotFound();
            }

            return View(strana);
        }

        // GET: stranas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: stranas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,strana_name")] strana strana)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(strana);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(strana);
        }

        // GET: stranas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var strana = await _context.strana.FindAsync(id);
            if (strana == null)
            {
                return NotFound();
            }
            return View(strana);
        }

        // POST: stranas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,strana_name")] strana strana)
        {
            if (id != strana.id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(strana);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!stranaExists(strana.id))
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
            return View(strana);
        }

        // GET: stranas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var strana = await _context.strana
                .FirstOrDefaultAsync(m => m.id == id);
            if (strana == null)
            {
                return NotFound();
            }

            return View(strana);
        }

        // POST: stranas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var strana = await _context.strana.FindAsync(id);
            if (strana != null)
            {
                _context.strana.Remove(strana);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool stranaExists(int id)
        {
            return _context.strana.Any(e => e.id == id);
        }
    }
}
