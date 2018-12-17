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
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Clients
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Client.Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.User)
                .Include(c => c.Projects)
                .Include(c => c.ClientArtists)
                    .ThenInclude(ca => ca.Artist)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();

            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();

            var artistListOptions = new List<SelectListItem>();

            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
                });
            }

            ClientCreateViewModel createViewModel = new ClientCreateViewModel();

            createViewModel.AvailableArtists = artistListOptions;

            return View(createViewModel);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateViewModel createClient)
        {
            ModelState.Remove("Client.User");
            ModelState.Remove("Client.UserId");
            if (ModelState.IsValid)
            {
                createClient.Client.User = await GetCurrentUserAsync();
                createClient.Client.UserId = createClient.Client.User.Id;
                _context.Add(createClient.Client);
                if (createClient.SelectedArtists != null)
                {
                    foreach (int artistId in createClient.SelectedArtists)
                    {
                        ClientArtist newCA = new ClientArtist()
                        {
                            ClientId = createClient.Client.ClientId,
                            ArtistId = artistId
                        };
                        _context.Add(newCA);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createClient);
        }

        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id, ClientEditViewModel editViewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();

            var artists = await _context.Artist.Where(a => a.User == user).ToListAsync();
            var clientArtists = await _context.ClientArtist.Where(ca => ca.ClientId == id).ToListAsync();

            var artistListOptions = new List<SelectListItem>();
            var artistIds = new List<int>();

            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
                });
            }

            foreach (ClientArtist ca in clientArtists)
            {
                artistIds.Add(ca.ArtistId);
            }

            editViewModel.AvailableArtists = artistListOptions;
            editViewModel.Client = client;
            editViewModel.SelectedArtists = artistIds;
            return View(editViewModel);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientEditViewModel editClient)
        {
            if (id != editClient.Client.ClientId)
            {
                return NotFound();
            }

            ModelState.Remove("Client.User");
            ModelState.Remove("Client.UserId");
            if (ModelState.IsValid)
            {
                // Delete joiner tables
                var clientArtists = await _context.ClientArtist.Where(ca => ca.ClientId == id).ToListAsync();
                if (clientArtists != null)
                {
                    foreach (ClientArtist ca in clientArtists)
                    {
                        _context.ClientArtist.Remove(ca);
                    }
                }
                try
                {
                    editClient.Client.User = await GetCurrentUserAsync();
                    editClient.Client.UserId = editClient.Client.User.Id;
                    // Add the new joiner tables if their are any
                    if (editClient.SelectedArtists != null)
                    {
                        foreach (int artistId in editClient.SelectedArtists)
                        {
                            ClientArtist newCA = new ClientArtist()
                            {
                                ClientId = editClient.Client.ClientId,
                                ArtistId = artistId
                            };
                            _context.Add(newCA);
                        }
                    }
                    _context.Update(editClient.Client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(editClient.Client.ClientId))
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
            return View(editClient);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Client client = await _context.Client
                .Include(c => c.ClientArtists)
                .SingleOrDefaultAsync(c => c.ClientId == id);
            //.FindAsync(id);

            foreach (ClientArtist ca in client.ClientArtists)
            {
                _context.ClientArtist.Remove(ca);
            }

            _context.Client.Remove(client);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
    }
}
