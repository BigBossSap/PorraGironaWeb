﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PorraGironaWeb.Models.Entity;


namespace PorraGironaWeb.Controllers
{
    public class PuntuacionsController : Controller
    {
        //private readonly PostDbContext _context;

        //public PuntuacionsController(PostDbContext context)
        //{
        //    _context = context;
        //}

        // GET: Puntuacions

        PostDbContext _context;
        public PuntuacionsController()
        {
            _context = new PostDbContext();
            
        }

        public async Task<IActionResult> Index()

            //si es admin o soci
        {
            if (NivellAcces() == 0 || NivellAcces() == 5)
            {
                List<Puntuacion> llista_puntuacions =_context.Puntuacions.FromSqlRaw("SELECT * FROM puntuacions").ToList();
                llista_puntuacions.RemoveAll(puntuacio_aux => puntuacio_aux.Puntuacio == 0);
                var llista_puntuacions_task = await Task.Run(() => llista_puntuacions);
                return View(llista_puntuacions_task.OrderByDescending(p=> p.Puntuacio));
               // return View(await _context.Puntuacions.OrderByDescending(p => p.Puntuacio).ToListAsync());
            }
            // si no redireccionem a la plana principal

            else
                
            return RedirectToAction("Index", "Login");
           

        }

        // GET: Puntuacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puntuacion = await _context.Puntuacions
                .FirstOrDefaultAsync(m => m.Idpuntuacio == id);
            if (puntuacion == null)
            {
                return NotFound();
            }

            return View(puntuacion);
        }

        // GET: Puntuacions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Puntuacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idpuntuacio,Idpenyista,Puntuacio,Temporada")] Puntuacion puntuacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(puntuacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(puntuacion);
        }

        // GET: Puntuacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puntuacion = await _context.Puntuacions.FindAsync(id);
            if (puntuacion == null)
            {
                return NotFound();
            }
            return View(puntuacion);
        }

        // POST: Puntuacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idpuntuacio,Idpenyista,Puntuacio,Temporada")] Puntuacion puntuacion)
        {
            if (id != puntuacion.Idpuntuacio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(puntuacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PuntuacionExists(puntuacion.Idpuntuacio))
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
            return View(puntuacion);
        }

        // GET: Puntuacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puntuacion = await _context.Puntuacions
                .FirstOrDefaultAsync(m => m.Idpuntuacio == id);
            if (puntuacion == null)
            {
                return NotFound();
            }

            return View(puntuacion);
        }

        // POST: Puntuacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var puntuacion = await _context.Puntuacions.FindAsync(id);
            _context.Puntuacions.Remove(puntuacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PuntuacionExists(int id)
        {
            return _context.Puntuacions.Any(e => e.Idpuntuacio == id);
        }

        private int NivellAcces()
        {
            int nivell = 10;

            String rol;
            byte[] valor = null;
            bool existeix = HttpContext.Session.TryGetValue("rol", out valor);
            if(valor!=null)
            {
                rol = System.Text.Encoding.UTF8.GetString(valor);

                if (rol == "admin")
                {
                    nivell = 0;
                }

                if (rol == "soci")
                {
                    nivell = 5;
                }
            }
           return nivell;
        }
    }
}
