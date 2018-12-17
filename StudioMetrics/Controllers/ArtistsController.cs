using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudioMetrics.Data;
using StudioMetrics.Models;
using StudioMetrics.Models.ViewModels;

namespace StudioMetrics.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArtistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Artists
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Artist.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Artists/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.User)
                .Include(a => a.ArtistProjects)
                    .ThenInclude(ap => ap.Project)
                .Include(a => a.ClientArtists)
                    .ThenInclude(ca => ca.Client)
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();

            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();

            var clientListOptions = new List<SelectListItem>();

            foreach (Client c in clients)
            {
                clientListOptions.Add(new SelectListItem
                {
                    Value = c.ClientId.ToString(),
                    Text = c.Name
                });
            }

            ArtistCreateViewModel createViewModel = new ArtistCreateViewModel();

            createViewModel.AvailableClients = clientListOptions;

            return View(createViewModel);
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtistCreateViewModel createArtist)
        {
            ModelState.Remove("Artist.User");
            ModelState.Remove("Artist.UserId");
            if (ModelState.IsValid)
            {
                createArtist.Artist.User = await GetCurrentUserAsync();
                createArtist.Artist.UserId = createArtist.Artist.User.Id;
                _context.Add(createArtist.Artist);
                if (createArtist.SelectedClients != null)
                {
                    foreach (int clientId in createArtist.SelectedClients)
                    {
                        ClientArtist newCA = new ClientArtist()
                        {
                            ClientId = clientId,
                            ArtistId = createArtist.Artist.ArtistId
                        };
                        _context.Add(newCA);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createArtist);
        }

        // GET: Artists/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id, ArtistEditViewModel editViewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();

            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var clientArtists = await _context.ClientArtist.Where(ca => ca.ArtistId == id).ToListAsync();

            var clientListOptions = new List<SelectListItem>();
            var clientIds = new List<int>();

            foreach (Client c in clients)
            {
                clientListOptions.Add(new SelectListItem
                {
                    Value = c.ClientId.ToString(),
                    Text = c.Name
                });
            }

            foreach (ClientArtist ca in clientArtists)
            {
                clientIds.Add(ca.ClientId);
            }

            editViewModel.AvailableClients = clientListOptions;
            editViewModel.Artist = artist;
            editViewModel.SelectedClients = clientIds;

            return View(editViewModel);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtistEditViewModel editArtist)
        {
            if (id != editArtist.Artist.ArtistId)
            {
                return NotFound();
            }

            ModelState.Remove("Artist.User");
            ModelState.Remove("Artist.UserId");
            if (ModelState.IsValid)
            {
                // Delete joiner tables
                var clientArtists = await _context.ClientArtist.Where(ca => ca.ArtistId == id).ToListAsync();
                if (clientArtists != null)
                {
                    foreach (ClientArtist ca in clientArtists)
                    {
                        _context.ClientArtist.Remove(ca);
                    }
                }
                try
                {
                    editArtist.Artist.User = await GetCurrentUserAsync();
                    editArtist.Artist.UserId = editArtist.Artist.User.Id;
                    // Add the new joiner tables if their are any
                    if (editArtist.SelectedClients != null)
                    {
                        foreach (int clientId in editArtist.SelectedClients)
                        {
                            ClientArtist newCA = new ClientArtist()
                            {
                                ClientId = clientId,
                                ArtistId = editArtist.Artist.ArtistId
                            };
                            _context.Add(newCA);
                        }
                    }
                    _context.Update(editArtist.Artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(editArtist.Artist.ArtistId))
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
            return View(editArtist);
        }

        // GET: Artists/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artist
                .SingleOrDefaultAsync(p => p.ArtistId == id);

            var artistProjects = await _context.ArtistProject.Where(ap => ap.ArtistId == id).ToListAsync();
            if (artistProjects != null)
            {
                foreach (ArtistProject ap in artistProjects)
                {
                    _context.ArtistProject.Remove(ap);
                }
            }
            var clientArtists = await _context.ClientArtist.Where(ap => ap.ArtistId == id).ToListAsync();
            if (clientArtists != null)
            {
                foreach (ClientArtist ca in clientArtists)
                {
                    _context.ClientArtist.Remove(ca);
                }
            }

            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artist.Any(e => e.ArtistId == id);
        }
    }
}
