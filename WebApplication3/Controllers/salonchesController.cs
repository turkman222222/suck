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
    public class salonchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public salonchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: salonches
        public async Task<IActionResult> Index()
        {
            return View(await _context.salonch.ToListAsync());
        }

        // GET: salonches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonch = await _context.salonch
                .FirstOrDefaultAsync(m => m.id == id);
            if (salonch == null)
            {
                return NotFound();
            }

            return View(salonch);
        }

        // GET: salonches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: salonches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,salon")] salonch salonch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salonch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salonch);
        }

        // GET: salonches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonch = await _context.salonch.FindAsync(id);
            if (salonch == null)
            {
                return NotFound();
            }
            return View(salonch);
        }

        // POST: salonches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,salon")] salonch salonch)
        {
            if (id != salonch.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salonch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!salonchExists(salonch.id))
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
            return View(salonch);
        }

        // GET: salonches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salonch = await _context.salonch
                .FirstOrDefaultAsync(m => m.id == id);
            if (salonch == null)
            {
                return NotFound();
            }

            return View(salonch);
        }

        // POST: salonches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salonch = await _context.salonch.FindAsync(id);
            if (salonch != null)
            {
                _context.salonch.Remove(salonch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool salonchExists(int id)
        {
            return _context.salonch.Any(e => e.id == id);
        }
    }
}
