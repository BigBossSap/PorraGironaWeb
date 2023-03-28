using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PorraGironaWeb.Models.Entity;

namespace PorraGironaWeb.Controllers
{
    public class EquipsController : Controller
    {
        //private readonly PostDbContext _context;

        //public EquipsController(PostDbContext context)
        //{
        //    _context = context;
        //}

        PostDbContext _context;
        public EquipsController()
        {
            _context = new PostDbContext();

        }

        // GET: Equips
        public async Task<IActionResult> Index()
        {
            List<Equip> llista_equips = _context.Equips.FromSqlRaw("SELECT * FROM equips").ToList();
            var llista_puntuacions_equips = await Task.Run(() => llista_equips);
            return View(llista_equips.ToList()); 
        }

      
        // GET: Equips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equip = await _context.Equips
                .FirstOrDefaultAsync(m => m.Idequip == id);
            if (equip == null)
            {
                return NotFound();
            }

            return View(equip);
        }

        // GET: Equips/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Equips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idequip,Nom,Imatge")] Equip equip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equip);
        }

        // GET: Equips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equip = await _context.Equips.FindAsync(id);
            if (equip == null)
            {
                return NotFound();
            }
            return View(equip);
        }

        // POST: Equips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idequip,Nom,Imatge")] Equip equip)
        {
            if (id != equip.Idequip)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipExists(equip.Idequip))
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
            return View(equip);
        }

        // GET: Equips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equip = await _context.Equips
                .FirstOrDefaultAsync(m => m.Idequip == id);
            if (equip == null)
            {
                return NotFound();
            }

            return View(equip);
        }

        // POST: Equips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equip = await _context.Equips.FindAsync(id);
            _context.Equips.Remove(equip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipExists(int id)
        {
            return _context.Equips.Any(e => e.Idequip == id);
        }
    }
}
