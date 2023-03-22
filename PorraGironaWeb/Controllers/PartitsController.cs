using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PorraGironaWeb.Models.Entity;
using System.Diagnostics;

namespace PorraGironaWeb.Controllers
{
    public class PartitsController : Controller
    {
        //private readonly PostDbContext _context;

        //public PartitsController(PostDbContext context)
        //{
        //    _context = context;
        //}

        PostDbContext _context;
        public PartitsController()
        {
            _context = new PostDbContext();

        }
        // GET: Partits
        public async Task<IActionResult> Index()
        {
            var partits = _context.Partits.Include(p => p.IdequiplocalNavigation).Include(p => p.IdequipvisitantNavigation).OrderBy(p=> p.Jornada);
            return View(await partits.ToListAsync());
        }

        // GET: Partits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partit = await _context.Partits
                .Include(p => p.IdequiplocalNavigation)
                .Include(p => p.IdequipvisitantNavigation)
                .FirstOrDefaultAsync(m => m.Idpartit == id);
            if (partit == null)
            {
                return NotFound();
            }

            return View(partit);
        }

        // GET: Partits/Create
        public IActionResult Create()
        {
            ViewData["Idequiplocal"] = new SelectList(_context.Equips, "Idequip", "Idequip");
            ViewData["Idequipvisitant"] = new SelectList(_context.Equips, "Idequip", "Idequip");
            return View();
        }

        // POST: Partits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idpartit,Idequiplocal,Idequipvisitant,Datainici,Jornada,Golslocal,Golsvisitant,Finalitzat,Temporada,Idsjugadorslocal,Idsjugadorsvisitant")] Partit partit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(partit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idequiplocal"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequiplocal);
            ViewData["Idequipvisitant"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequipvisitant);
            return View(partit);
        }

        // GET: Partits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partit = await _context.Partits.FindAsync(id);
            if (partit == null)
            {
                return NotFound();
            }

            ViewData["Finalitzat"] = new SelectList(new List<Object>
            {
                new {value = "SI", text = "SI"},
                new {value = "NO", text = "NO"}, },
            "value", "text");
           
            ViewData["Idequiplocal"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequiplocal);
            ViewData["Idequipvisitant"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequipvisitant);
            return View(partit);
        }

        // POST: Partits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idpartit,Idequiplocal,Idequipvisitant,Datainici,Jornada,Golslocal,Golsvisitant,Finalitzat,Temporada,Idsjugadorslocal,Idsjugadorsvisitant")] Partit p_partit)
        {
            if (id != p_partit.Idpartit)
            {
                return NotFound();
            }
            Partit partit = _context.Partits.Find(p_partit.Idpartit);
            partit.Datainici = p_partit.Datainici;
            partit.Finalitzat = p_partit.Finalitzat;
            partit.Golslocal = p_partit.Golslocal;
            partit.Golslocal = p_partit.Golsvisitant;            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partit);
                    await _context.SaveChangesAsync();
                    CalcularPuntuacioEntity();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartitExists(partit.Idpartit))
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
            ViewData["Idequiplocal"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequiplocal);
            ViewData["Idequipvisitant"] = new SelectList(_context.Equips, "Idequip", "Idequip", partit.Idequipvisitant);
            return View(partit);
        }

        // GET: Partits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partit = await _context.Partits
                .Include(p => p.IdequiplocalNavigation)
                .Include(p => p.IdequipvisitantNavigation)
                .FirstOrDefaultAsync(m => m.Idpartit == id);
            if (partit == null)
            {
                return NotFound();
            }

            return View(partit);
        }

        // POST: Partits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partit = await _context.Partits.FindAsync(id);
            _context.Partits.Remove(partit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartitExists(int id)
        {
            return _context.Partits.Any(e => e.Idpartit == id);
        }


        public Boolean CalcularPuntuacioEntity()
        {
            PostDbContext context = new PostDbContext();
            List<Partit> llista_partits;
            Boolean result = true;
            Puntuacion puntuacio;

            try
            {
                int registers_affected = context.Database.ExecuteSqlRaw("TRUNCATE TABLE puntuacions");

                List<Penyiste> llistatPenyistes = new List<Penyiste>();
                llistatPenyistes = context.Penyistes.ToList<Penyiste>();

                Penyiste penyista;
                for (int i = 0; i < llistatPenyistes.Count(); i++)
                {
                    penyista = llistatPenyistes[i];
                    List<Porre> llistatPorres = new List<Porre>();
                    llistatPorres = context.Porres.Where(porra_aux => porra_aux.Idpenyista == penyista.Idpenyista).ToList<Porre>();
                    puntuacio = new Puntuacion();
                    puntuacio.Idpenyista = penyista.Idpenyista;
                    puntuacio.Alias = penyista.Alias;
                    puntuacio.Puntuacio = 0;

                    Porre porra;

                    for (int j = 0; j < llistatPorres.Count(); j++)
                    {

                        porra = llistatPorres[j];
                        llista_partits = context.Partits.Where(partit_aux => partit_aux.Idpartit == porra.Idpartit
                         && partit_aux.Finalitzat.Equals("SI")).ToList<Partit>();

                        if (llista_partits != null)
                        {
                            if (llista_partits.Count > 0)
                            {
                                Partit partits = llista_partits[0];
                                puntuacio.Puntuacio = puntuacio.Puntuacio + CalcularPuntsPorra(porra, partits);
                            }
                        }
                    }
                    context.Puntuacions.Add(puntuacio);
                }
               result = result && (context.SaveChanges() > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        public int CalcularPuntsPorra(Porre porra, Partit partit)
        {
            int punts = 0;
            int guanyador = 0;
            int guanyadorPorra = 0;
            if (partit.Golsvisitant > partit.Golslocal)
                guanyador = 2;
            else
                guanyador = 1;
            if (porra.Golsvisitant > porra.Golslocal)
                guanyadorPorra = 2;
            else
                guanyadorPorra = 1;

            if (porra.Golslocal == partit.Golslocal && porra.Golsvisitant == partit.Golsvisitant)
                punts = 5;

            else if (porra.Golslocal == partit.Golslocal + 1 || porra.Golsvisitant == partit.Golsvisitant + 1)
                punts = 2;

            else if (porra.Golslocal == partit.Golslocal && porra.Golsvisitant != partit.Golsvisitant)
                punts = 4;

            else if (porra.Golslocal != partit.Golslocal && porra.Golsvisitant == partit.Golsvisitant)
                punts = 4;
            else if (guanyadorPorra == guanyador)
                punts = 3;

            return punts;

        }
    }
}
