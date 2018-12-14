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
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Project.Include(p => p.Client).Include(p => p.ProjectType).Include(p => p.StatusType).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Client)
                .Include(p => p.ProjectType)
                .Include(p => p.StatusType)
                .Include(p => p.PlayerProjects)
                    .ThenInclude(pp => pp.Player)
                .Include(p => p.ArtistProjects)
                    .ThenInclude(ap => ap.Artist)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();

            var projectTypes = await _context.ProjectType.ToListAsync();
            var statusTypes = await _context.StatusType.ToListAsync();
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();

            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();

            foreach (ProjectType pt in projectTypes)
            {
                projectTypeListOptions.Add(new SelectListItem
                {
                    Value = pt.ProjectTypeId.ToString(),
                    Text = pt.Type
                });
            }

            foreach (StatusType st in statusTypes)
            {
                statusTypeListOptions.Add(new SelectListItem
                {
                    Value = st.StatusTypeId.ToString(),
                    Text = st.Type
                });
            }

            foreach (Client c in clients)
            {
                clientListOptions.Add(new SelectListItem
                {
                    Value = c.ClientId.ToString(),
                    Text = c.Name
                });
            }

            ProjectCreateViewModel createViewModel = new ProjectCreateViewModel();

            projectTypeListOptions.Insert(0, new SelectListItem
            {
                Text = "Choose a Project Type",
                Value = "0"
            });

            statusTypeListOptions.Insert(0, new SelectListItem
            {
                Text = "Choose a Status",
                Value = "0"
            });

            clientListOptions.Insert(0, new SelectListItem
            {
                Text = "Choose a Client",
                Value = "0"
            });

            createViewModel.ProjectTypes = projectTypeListOptions;
            createViewModel.StatusTypes = statusTypeListOptions;
            createViewModel.Clients = clientListOptions;

            return View(createViewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Title,ProjectTypeId,Description,Payrate,TimeTable,StartDate,StatusTypeId,ClientId,UserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", project.ClientId);
            ViewData["ProjectTypeId"] = new SelectList(_context.ProjectType, "ProjectTypeId", "Type", project.ProjectTypeId);
            ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "StatusTypeId", "Type", project.StatusTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.UserId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", project.ClientId);
            ViewData["ProjectTypeId"] = new SelectList(_context.ProjectType, "ProjectTypeId", "Type", project.ProjectTypeId);
            ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "StatusTypeId", "Type", project.StatusTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.UserId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Title,ProjectTypeId,Description,Payrate,TimeTable,StartDate,StatusTypeId,ClientId,UserId")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
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
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", project.ClientId);
            ViewData["ProjectTypeId"] = new SelectList(_context.ProjectType, "ProjectTypeId", "Type", project.ProjectTypeId);
            ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "StatusTypeId", "Type", project.StatusTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", project.UserId);
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Client)
                .Include(p => p.ProjectType)
                .Include(p => p.StatusType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }
    }
}
