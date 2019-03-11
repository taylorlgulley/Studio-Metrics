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
    // Inheriting the Controller class
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Dependency Injection for the ApplicationDbContext and UserManager
        public ArtistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // Retrieving the current user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Artists
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Artist.Where(a => a.User == user).Include(a => a.User);
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

            // Retrieve the artist from the database and include both the ArtistProjects and ClientArtists. Additionally attach the Project and Client respectively to display this information on the details page.
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

            // Retrieve all the clients associated with the current user from the database.
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();

            // Create a list of SelectListItems to hold the client options
            var clientListOptions = new List<SelectListItem>();

            // Loop over the clients to create the SelectListItem and add it to the list
            foreach (Client c in clients)
            {
                clientListOptions.Add(new SelectListItem
                {
                    Value = c.ClientId.ToString(),
                    Text = c.Name
                });
            }

            // Create an instance of the ArtistCreateViewModel to add the list options to 
            ArtistCreateEditViewModel createViewModel = new ArtistCreateEditViewModel();
            
            // Set the client list options ot the AvailableClients on the view model
            createViewModel.AvailableClients = clientListOptions;

            return View(createViewModel);
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtistCreateEditViewModel createArtist)
        {
            // Remove the User and UserId so the ModelState can be valid
            ModelState.Remove("Artist.User");
            ModelState.Remove("Artist.UserId");
            if (ModelState.IsValid)
            {
                // Attach the current user to the created artist and add them to the database
                createArtist.Artist.User = await GetCurrentUserAsync();
                createArtist.Artist.UserId = createArtist.Artist.User.Id;
                _context.Add(createArtist.Artist);
                // If the created artist has any SelectedClients foreach over it to create the individual ClientArtist and add it to the database.
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
                // Once everything has been added save the changes to the database.
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createArtist);
        }

        // GET: Artists/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id, ArtistCreateEditViewModel editViewModel)
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

            // Retrieve the Clients and ClientArtists associated with the user and artist respectively.
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var clientArtists = await _context.ClientArtist.Where(ca => ca.ArtistId == id).ToListAsync();

            // Create lists for the client options and the client ids of those already selected
            var clientListOptions = new List<SelectListItem>();
            var clientIds = new List<int>();

            // Loop over the retrieved clients and existing ClientArtists to add their respective SelectListListItem and ids
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

            // Set the lists to the view model
            editViewModel.AvailableClients = clientListOptions;
            editViewModel.SelectedClients = clientIds;
            editViewModel.Artist = artist;

            return View(editViewModel);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtistCreateEditViewModel editArtist)
        {
            if (id != editArtist.Artist.ArtistId)
            {
                return NotFound();
            }

            // Remove the User and UserId so the ModelState can be valid
            ModelState.Remove("Artist.User");
            ModelState.Remove("Artist.UserId");
            if (ModelState.IsValid)
            {
                // Delete joiner tables associated with the Artist before the edit so that there will not be duplicate tables after the edit is submitted.
                // First retrieve the ClientArtists from the database then loop over them to remove each one.
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
                    // Add the current user to the edited artist
                    editArtist.Artist.User = await GetCurrentUserAsync();
                    editArtist.Artist.UserId = editArtist.Artist.User.Id;
                    // Add the new joiner tables to the database if there are any. If there areany SelectedClients loop over them to create the ClientArtist and add it to the database.
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
                    // Update the database with the new artist and save the changes
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

            // Retrieve the Artist that has the id that was passed in
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
            // Retrieve the Artist from the database that has the id that was passed in.
            var artist = await _context.Artist
                .SingleOrDefaultAsync(p => p.ArtistId == id);
            
            // Retrieve the ArtistProjects and ClientArtists from the database that are associated with the Artist. Loop over both of these to remove these from the database before deleting the artist.
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

            // After removing the relationships with the Artist then remove the Artist from the database and save the changes.
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
