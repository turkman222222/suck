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
    public class users1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public users1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: users1
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.user.Include(u => u.rol);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: users1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.user
                .Include(u => u.rol)
                .FirstOrDefaultAsync(m => m.id_user == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: users1/Create
        public IActionResult Create()
        {
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id");
            return View();
        }

        // POST: users1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }

        // GET: users1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.user.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }

        // POST: users1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (id != user.id_user)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!userExists(user.id_user))
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
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }

        // GET: users1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.user
                .Include(u => u.rol)
                .FirstOrDefaultAsync(m => m.id_user == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: users1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.user.FindAsync(id);
            if (user != null)
            {
                _context.user.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool userExists(int id)
        {
            return _context.user.Any(e => e.id_user == id);
        }
    }
}
