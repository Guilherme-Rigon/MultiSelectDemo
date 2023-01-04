using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiSelectDemo.Data;
using MultiSelectDemo.Models;

namespace MultiSelectDemo.Controllers
{
    public class FilmesController : Controller
    {
        private readonly MultiSelectDemoDbContext _context;

        public FilmesController(MultiSelectDemoDbContext context)
        {
            _context = context;
        }

        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Filmes == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.FilmeId == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.AsNoTracking().Select(x => new SelectListItem(x.Nome, x.CategoriaId.ToString())).AsEnumerable();
            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmeId,Nome")] Filme filme, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var categorias = form["categorias"].ToString().Split(",").ToList();
                filme.Categorias = new List<Categoria>();
                categorias.ForEach(x =>
                {
                    filme.Categorias.Add(_context.Categorias.Find(Convert.ToInt32(x)));
                });
                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Filmes == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.Include(x => x.Categorias).AsNoTracking().FirstOrDefaultAsync(x => x.FilmeId == id);
            if (filme == null)
            {
                return NotFound();
            }
            var categorias = _context.Categorias.AsNoTracking().Select(x => new SelectListItem(x.Nome, x.CategoriaId.ToString())).ToList();
            categorias.ForEach(x =>
            {
                if (filme.Categorias.Any(y => y.CategoriaId.ToString() == x.Value))
                    x.Selected = true;
            });

            ViewBag.Categorias = categorias;
            return View(filme);
        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmeId,Nome")] Filme filme, IFormCollection form)
        {
            if (id != filme.FilmeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filme);

                    var categorias = form["categorias"].ToString().Split(",").ToList();
                    if (categorias[0] != string.Empty)
                    {
                        var tempCategorias = _context.Filmes.Include(x => x.Categorias).FirstOrDefault(x => x.FilmeId == filme.FilmeId);
                        // Adicionando as não inseridas
                        categorias.ForEach(x =>
                        {
                            if (!tempCategorias.Categorias.Any(y => y.CategoriaId.ToString() == x))
                            {
                                tempCategorias.Categorias.Add(_context.Categorias.Find(Convert.ToInt32(x)));
                            }
                        });
                        // Removendo as deletadas
                        foreach (Categoria cat in tempCategorias.Categorias.ToList())
                        {
                            if (!categorias.Any(x => x == cat.CategoriaId.ToString()))
                            {
                                tempCategorias.Categorias.Remove(cat);
                            }
                        }

                        _context.Update(tempCategorias);
                    }
                    else
                    {
                        ViewData["Error"] = "Deve haver ao menos uma categoria selecionada.";
                        return View(filme);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.FilmeId))
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
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Filmes == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.FilmeId == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Filmes == null)
            {
                return Problem("Entity set 'MultiSelectDemoDbContext.Filmes'  is null.");
            }
            var filme = await _context.Filmes.FindAsync(id);
            if (filme != null)
            {
                _context.Filmes.Remove(filme);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.FilmeId == id);
        }
    }
}
