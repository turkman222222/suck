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
    public class complsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public complsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: compls
        public async Task<IActionResult> Index()
        {
            return View(await _context.compl.ToListAsync());
        }

        // GET: compls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compl = await _context.compl
                .FirstOrDefaultAsync(m => m.id == id);
            if (compl == null)
            {
                return NotFound();
            }

            return View(compl);
        }

        // GET: compls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: compls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,kompl_name")] compl compl)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(compl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(compl);
        }

        // GET: compls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compl = await _context.compl.FindAsync(id);
            if (compl == null)
            {
                return NotFound();
            }
            return View(compl);
        }

        // POST: compls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,kompl_name")] compl compl)
        {
            if (id != compl.id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(compl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!complExists(compl.id))
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
            return View(compl);
        }

        // GET: compls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compl = await _context.compl
                .FirstOrDefaultAsync(m => m.id == id);
            if (compl == null)
            {
                return NotFound();
            }

            return View(compl);
        }

        // POST: compls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compl = await _context.compl.FindAsync(id);
            if (compl != null)
            {
                _context.compl.Remove(compl);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool complExists(int id)
        {
            return _context.compl.Any(e => e.id == id);
        }
    }
}
