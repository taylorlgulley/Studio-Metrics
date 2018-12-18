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

        // GET: Projects of certain type
        [Authorize]
        public async Task<IActionResult> ProjectsOfStatus(int ? id)
        {
            var projects = await _context.Project.Include(p => p.Client).Include(p => p.ProjectType).Include(p => p.StatusType).ToListAsync();

            if (id != null)
            {
                var filteredProjects = projects.Where(p => p.StatusTypeId == id).ToList();
                return View(filteredProjects);
                //var filteredProjects = await _context.Project.Where(p => p.StatusTypeId == id).ToListAsync();
            }
            return View(projects);
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
            var players = await _context.Player.Where(c => c.User == user).ToListAsync();
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();

            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();
            var playerListOptions = new List<SelectListItem>();
            var artistListOptions = new List<SelectListItem>();

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

            foreach (Player p in players)
            {
                playerListOptions.Add(new SelectListItem
                {
                    Value = p.PlayerId.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
            }

            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
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
            createViewModel.AvailablePlayers = playerListOptions;
            createViewModel.AvailableArtists = artistListOptions;

            return View(createViewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel createProject)
        {
            ModelState.Remove("Project.User");
            ModelState.Remove("Project.UserId");
            if (ModelState.IsValid)
            {
                createProject.Project.User = await GetCurrentUserAsync();
                createProject.Project.UserId = createProject.Project.User.Id;
                _context.Add(createProject.Project);
                // A foreach looping through the SelectedPlayers to create the instances of the PlayerProjects for every one in the list. Then adding them to the database. The foreach works because each entry in the list is an int representing the playerId then it takes the project Id from the created project in the model to make the StudentExercise. Needs to be in an if statement so a person can create a project and not assign them an project.
                if (createProject.SelectedPlayers != null)
                {
                    foreach (int playerId in createProject.SelectedPlayers)
                    {
                        PlayerProject newPP = new PlayerProject()
                        {
                            PlayerId = playerId,
                            ProjectId = createProject.Project.ProjectId
                        };
                        _context.Add(newPP);
                    }
                }
                if (createProject.SelectedArtists != null)
                {
                    foreach (int artistId in createProject.SelectedArtists)
                    {
                        ArtistProject newAP = new ArtistProject()
                        {
                            ArtistId = artistId,
                            ProjectId = createProject.Project.ProjectId
                        };
                        _context.Add(newAP);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var user = await GetCurrentUserAsync();

            var projectTypes = await _context.ProjectType.ToListAsync();
            var statusTypes = await _context.StatusType.ToListAsync();
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var players = await _context.Player.Where(c => c.User == user).ToListAsync();
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();

            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();
            var playerListOptions = new List<SelectListItem>();
            var artistListOptions = new List<SelectListItem>();

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

            foreach (Player p in players)
            {
                playerListOptions.Add(new SelectListItem
                {
                    Value = p.PlayerId.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
            }

            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
                });
            }

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

            createProject.ProjectTypes = projectTypeListOptions;
            createProject.StatusTypes = statusTypeListOptions;
            createProject.Clients = clientListOptions;
            createProject.AvailablePlayers = playerListOptions;
            createProject.AvailableArtists = artistListOptions;

            return View(createProject);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id, ProjectEditViewModel editProject)
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
            var user = await GetCurrentUserAsync();

            var projectTypes = await _context.ProjectType.ToListAsync();
            var statusTypes = await _context.StatusType.ToListAsync();
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var players = await _context.Player.Where(c => c.User == user).ToListAsync();
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();
            var playerProjects = await _context.PlayerProject.Where(c => c.ProjectId == id).ToListAsync();
            var artistProjects = await _context.ArtistProject.Where(c => c.ProjectId == id).ToListAsync();

            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();
            var playerListOptions = new List<SelectListItem>();
            var artistListOptions = new List<SelectListItem>();
            var playerIds = new List<int>();
            var artistIds = new List<int>();

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

            foreach (Player p in players)
            {
                playerListOptions.Add(new SelectListItem
                {
                    Value = p.PlayerId.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
            }

            foreach (Artist a in artists)
            {
                artistListOptions.Add(new SelectListItem
                {
                    Value = a.ArtistId.ToString(),
                    Text = a.Name
                });
            }

            foreach (PlayerProject pp in playerProjects)
            {
                playerIds.Add(pp.PlayerId);
            }

            foreach (ArtistProject ap in artistProjects)
            {
                artistIds.Add(ap.ArtistId);
            }

            editProject.ProjectTypes = projectTypeListOptions;
            editProject.StatusTypes = statusTypeListOptions;
            editProject.Clients = clientListOptions;
            editProject.AvailablePlayers = playerListOptions;
            editProject.AvailableArtists = artistListOptions;
            editProject.Project = project;
            editProject.SelectedPlayers = playerIds;
            editProject.SelectedArtists = artistIds;

            return View(editProject);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectEditViewModel editProject)
        {
            if (id != editProject.Project.ProjectId)
            {
                return NotFound();
            }

            ModelState.Remove("Project.User");
            ModelState.Remove("Project.UserId");
            if (ModelState.IsValid)
            {
                // Delete joiner tables
                var playerProjects = await _context.PlayerProject.Where(pp => pp.ProjectId == id).ToListAsync();
                if (playerProjects != null)
                {
                    foreach (PlayerProject pp in playerProjects)
                    {
                        _context.PlayerProject.Remove(pp);
                    }
                }
                var artistProjects = await _context.ArtistProject.Where(ap => ap.ProjectId == id).ToListAsync();
                if (artistProjects != null)
                {
                    foreach (ArtistProject ap in artistProjects)
                    {
                        _context.ArtistProject.Remove(ap);
                    }
                }
                try
                {
                    editProject.Project.User = await GetCurrentUserAsync();
                    editProject.Project.UserId = editProject.Project.User.Id;
                    // Add the new joiner tables if their are any
                    if (editProject.SelectedPlayers != null)
                    {
                        foreach (int playerId in editProject.SelectedPlayers)
                        {
                            PlayerProject newPP = new PlayerProject()
                            {
                                PlayerId = playerId,
                                ProjectId = editProject.Project.ProjectId
                            };
                            _context.Add(newPP);
                        }
                    }
                    if (editProject.SelectedArtists != null)
                    {
                        foreach (int artistId in editProject.SelectedArtists)
                        {
                            ArtistProject newAP = new ArtistProject()
                            {
                                ArtistId = artistId,
                                ProjectId = editProject.Project.ProjectId
                            };
                            _context.Add(newAP);
                        }
                    }
                    _context.Update(editProject.Project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(editProject.Project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                // Add logic to get the dropdown items and multiselect items
            }
            return View(editProject);
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
            var playerProjects = await _context.PlayerProject.Where(pp => pp.ProjectId == id).ToListAsync();
            if (playerProjects != null)
            {
                foreach (PlayerProject pp in playerProjects)
                {
                    _context.PlayerProject.Remove(pp);
                }
            }
            var artistProjects = await _context.ArtistProject.Where(ap => ap.ProjectId == id).ToListAsync();
            if (artistProjects != null)
            {
                foreach (ArtistProject ap in artistProjects)
                {
                    _context.ArtistProject.Remove(ap);
                }
            }
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
