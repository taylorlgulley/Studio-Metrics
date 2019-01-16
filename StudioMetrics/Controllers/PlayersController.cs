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

namespace StudioMetrics.Controllers
{
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Players
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Player.Where(p => p.User == user).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Players/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // find the PlayerProjects associated with the Player and then attach the Project so you have access to that information
            var player = await _context.Player
                .Include(p => p.User)
                .Include(p => p.PlayerProjects)
                    .ThenInclude(pp => pp.Project)
                .FirstOrDefaultAsync(m => m.PlayerId == id);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerId,FirstName,LastName,Instrument,Phone,Email,UserId")] Player player)
        {
            // Remove the User and UserId so that the ModelState can be Valid
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                // Attach the current Users to the created player and add to the database
                player.User = await GetCurrentUserAsync();
                player.UserId = player.User.Id;
                _context.Add(player);
                // Save the added player to the database
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlayerId,FirstName,LastName,Instrument,Phone,Email,UserId")] Player player)
        {
            if (id != player.PlayerId)
            {
                return NotFound();
            }

            // Remove the User and UserId so the ModelState can be valid
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the current user to the edited player and update that player in the database. Then save the changes.
                    player.User = await GetCurrentUserAsync();
                    player.UserId = player.User.Id;
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.PlayerId))
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
            return View(player);
        }

        // GET: Players/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the player from the database with the id matching the one passed in
            var player = await _context.Player
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Need to make sure that if the player has PlayerProjects that they are also deleted when the player is deleted
            Player player = await _context.Player
                .SingleOrDefaultAsync(p => p.PlayerId == id);

            // Retrieve the PlayerProjects from the database that are associated with the player being deleted
            var playerProjects = await _context.PlayerProject.Where(pp => pp.PlayerId == id).ToListAsync();
            // If the retrieved PlayerProjects are not null then foreach through the list and remove each PlayerProject from the database.
            // This is to make sure that when a player is deleted their are no PlayerProjects left over referencing the deleted player.
            if (playerProjects != null)
            {
                foreach (PlayerProject pp in playerProjects)
                {
                    _context.PlayerProject.Remove(pp);
                }
            }

            // After having removed all the PlayerProjects associated with the player. Then remove the player from the database
            _context.Player.Remove(player);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Player.Any(e => e.PlayerId == id);
        }
    }
}
