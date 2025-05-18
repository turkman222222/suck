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
    public class cvetasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public cvetasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: cvetas
        public async Task<IActionResult> Index()
        {
            return View(await _context.cveta.ToListAsync());
        }

        // GET: cvetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cveta = await _context.cveta
                .FirstOrDefaultAsync(m => m.id == id);
            if (cveta == null)
            {
                return NotFound();
            }

            return View(cveta);
        }

        // GET: cvetas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: cvetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,cvet_name")] cveta cveta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cveta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cveta);
        }

        // GET: cvetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cveta = await _context.cveta.FindAsync(id);
            if (cveta == null)
            {
                return NotFound();
            }
            return View(cveta);
        }

        // POST: cvetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,cvet_name")] cveta cveta)
        {
            if (id != cveta.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cveta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!cvetaExists(cveta.id))
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
            return View(cveta);
        }

        // GET: cvetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cveta = await _context.cveta
                .FirstOrDefaultAsync(m => m.id == id);
            if (cveta == null)
            {
                return NotFound();
            }

            return View(cveta);
        }

        // POST: cvetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cveta = await _context.cveta.FindAsync(id);
            if (cveta != null)
            {
                _context.cveta.Remove(cveta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool cvetaExists(int id)
        {
            return _context.cveta.Any(e => e.id == id);
        }
    }
}
