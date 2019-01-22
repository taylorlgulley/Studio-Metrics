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
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Client.Where(c => c.User == user).Include(c => c.User);
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

            // Retrieve the single client from the database that has the id matching the one passed in. Include the information about Artists for the ClientArtists
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
            // Retrieve all the artists associated with the current user to be used to create the options
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();
            // Create a list of SelectListItems to hold the options for artists
            var artistListOptions = new List<SelectListItem>();
            // Loop through the retireved artists to created each option with the value being the id and the text being the name
            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
                });
            }
            // Create an instance of the ClientCreateView Model to then add the options to 
            ClientCreateViewModel createViewModel = new ClientCreateViewModel();
            // Set the artist list options you created to the AvailableArtists in the view model
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
            // Remove the User and UserId so the ModelState can be valid
            ModelState.Remove("Client.User");
            ModelState.Remove("Client.UserId");
            if (ModelState.IsValid)
            {
                // Add the current user to the created client. Then add that client to the database.
                createClient.Client.User = await GetCurrentUserAsync();
                createClient.Client.UserId = createClient.Client.User.Id;
                _context.Add(createClient.Client);
                // If the created client has any selected artists. Then loop over them and create a ClientArtist for each one as well as add them to the database.
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
                // Save all the changes made to the database
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

            // Retrieve the Client from the database that has the id passed in
            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();

            //Retrieve all the artists associated with the user and the ClientArtists associated with the client
            var artists = await _context.Artist.Where(a => a.User == user).ToListAsync();
            var clientArtists = await _context.ClientArtist.Where(ca => ca.ClientId == id).ToListAsync();

            // Make a list to hold the options for artists that are available and the selected artists
            var artistListOptions = new List<SelectListItem>();
            var artistIds = new List<int>();

            // Loop over the retrieved artists and ClientArtists to add to their respective lists
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

            // Set the lists you created and filled with the appropriate information to the view model
            editViewModel.AvailableArtists = artistListOptions;
            editViewModel.SelectedArtists = artistIds;
            editViewModel.Client = client;
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

            // Remove the User and UserId so the ModelState can be Valid
            ModelState.Remove("Client.User");
            ModelState.Remove("Client.UserId");
            if (ModelState.IsValid)
            {
                // Delete the joiner tables associated with the client. Retrieve all of the ClientArtists from the database then loop over them and remove each one. This is so you can add the joiner tables from the edited client and not have duplicates.
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
                    // Add the current user
                    editClient.Client.User = await GetCurrentUserAsync();
                    editClient.Client.UserId = editClient.Client.User.Id;
                    // Add the new joiner tables if their are any from the edited client. Foreach over the selected artists and add the ClientArtists to the database
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
                    // Update the database with the edited client and save the changes.
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

            // Retrieve the client with the id matching the one passed in
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
            // Retrieve the client from the database that matches the id that was passed in
            Client client = await _context.Client
                .Include(c => c.ClientArtists)
                .SingleOrDefaultAsync(c => c.ClientId == id);
            // Loop over each of the included ClientArtists and remove them from the database
            foreach (ClientArtist ca in client.ClientArtists)
            {
                _context.ClientArtist.Remove(ca);
            }

            // Once each ClientArtist is removed then remove the client from the database and save the changes.
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
