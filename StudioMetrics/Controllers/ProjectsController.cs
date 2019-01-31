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

        // GET: Search Projects
        [Authorize]
        public async Task<IActionResult> Search(string search)
        {
            // Creating an instance of the view model for the search feature
            ProjectSearchViewModel viewmodel = new ProjectSearchViewModel();
            //Retrieving the user that is currently logged in
            var user = await GetCurrentUserAsync();
            // Setting the string from the search bar as the Search property in the view model for the search view
            viewmodel.Search = search;
            // Retrieving all the projects from the database that have the current user attached to the project and that have a title containing the search parameter. These projects will be displayed in a table for the search view
            viewmodel.Projects = await _context.Project
                                    .Where(p => p.Title.Contains(search) && p.User == user)
                                    .ToListAsync();

            return View(viewmodel);
        }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Project.Where(p => p.User == user).Include(p => p.Client).Include(p => p.ProjectType).Include(p => p.StatusType).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects of certain type
        [Authorize]
        public async Task<IActionResult> ProjectsOfStatus(int ? id)
        {
            // Get the user that is currently logged in
            var user = await GetCurrentUserAsync();
            // Retrieve all the projects fromthe database that have the current user and include all the client, project type, and status type
            var projects = await _context.Project.Where(p => p.User == user).Include(p => p.Client).Include(p => p.ProjectType).Include(p => p.StatusType).ToListAsync();

            // If the id that is passed in is not null then use the id to filter the projects
            // Only showing the projects that have a status type id matching the id passed in
            if (id != null)
            {
                var filteredProjects = projects.Where(p => p.StatusTypeId == id).ToList();
                return View(filteredProjects);
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

            // Retrieving the single project from the database that has the matching ProjectId to the id passed in
            // Include all the information needed for the project model like client, project type etc.
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

            // These are retrieving data from the database to populate the dropdown lists
            var projectTypes = await _context.ProjectType.ToListAsync();
            var statusTypes = await _context.StatusType.ToListAsync();
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var players = await _context.Player.Where(c => c.User == user).ToListAsync();
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();

            // Creating lists of SelectListItems to hold the data retrieved above for the ProjectCreateViewModel
            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();
            var playerListOptions = new List<SelectListItem>();
            var artistListOptions = new List<SelectListItem>();

            // Do a foreach over the set of data to add each individual list item to their respective list, where the value is the id and the text is the name or type
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

            // Create an instance of the ProjectCreateViewModel to add the created list options to
            ProjectCreateEditViewModel createViewModel = new ProjectCreateEditViewModel();

            // Insert a select list item in the first position so that the dropdown options have a placeholder for required fields
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

            // Set the created lists to the view model
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
        public async Task<IActionResult> Create(ProjectCreateEditViewModel createProject)
        {
            // Must remove the User and User Id from the Project for the Model State to be valid (these may have been placeholder values)
            ModelState.Remove("Project.User");
            ModelState.Remove("Project.UserId");
            if (ModelState.IsValid)
            {
                // Add the current User and UserId to the project
                createProject.Project.User = await GetCurrentUserAsync();
                createProject.Project.UserId = createProject.Project.User.Id;
                // Add the newly created project to the database
                _context.Add(createProject.Project);
                // A foreach looping through the SelectedPlayers to create the instances of the PlayerProjects for every one in the list. Then adding them to the database. The foreach works because each entry in the list is an int representing the playerId then it takes the project Id from the created project in the model to make the PlayerProject. Needs to be in an if statement so a person can create a project and not assign players to a project. Then this same thing is done for the SelectedArtists
                if (createProject.SelectedPlayers != null)
                {
                    foreach (int playerId in createProject.SelectedPlayers)
                    {
                        PlayerProject newPP = new PlayerProject()
                        {
                            PlayerId = playerId,
                            ProjectId = createProject.Project.ProjectId
                        };
                        // Add each individual Player Project to the database
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
                // Save all the changes to the database. The additions to the database will not completed until they are saved. This is similar to git like adding changes to staging before making a commit.
                await _context.SaveChangesAsync();
                // Redirect to the projects being filtered to show only the the Current project.
                return RedirectToAction("ProjectsofStatus", new { id = 3 });
            }

            // All of this logic is adding the list options back to the create view model so that if the model state is not valid it will return to the create view and maintain the lists. If this is not done when it returns to the view the lists will be empty because the lists are not being held. This logic comes from the get portion of the create.
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
        public async Task<IActionResult> Edit(int? id, ProjectCreateEditViewModel editProject)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the project in the database that has the passed in id as the primary key for the Project
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();

            // Gather the data from the database to populate the lists to be added to the edit view model
            var projectTypes = await _context.ProjectType.ToListAsync();
            var statusTypes = await _context.StatusType.ToListAsync();
            var clients = await _context.Client.Where(c => c.User == user).ToListAsync();
            var players = await _context.Player.Where(c => c.User == user).ToListAsync();
            var artists = await _context.Artist.Where(c => c.User == user).ToListAsync();
            var playerProjects = await _context.PlayerProject.Where(c => c.ProjectId == id).ToListAsync();
            var artistProjects = await _context.ArtistProject.Where(c => c.ProjectId == id).ToListAsync();

            // Create lists to be filled the information obtained above
            var projectTypeListOptions = new List<SelectListItem>();
            var statusTypeListOptions = new List<SelectListItem>();
            var clientListOptions = new List<SelectListItem>();
            var playerListOptions = new List<SelectListItem>();
            var artistListOptions = new List<SelectListItem>();
            var playerIds = new List<int>();
            var artistIds = new List<int>();

            // Do a foreach over the set of data to add each individual list item to their respective list, where the value is the id and the text is the name or type
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

            // These fill player and artist id lists 
            foreach (PlayerProject pp in playerProjects)
            {
                playerIds.Add(pp.PlayerId);
            }

            foreach (ArtistProject ap in artistProjects)
            {
                artistIds.Add(ap.ArtistId);
            }

            // Set the lists of options to the project edit view model
            editProject.ProjectTypes = projectTypeListOptions;
            editProject.StatusTypes = statusTypeListOptions;
            editProject.Clients = clientListOptions;
            editProject.AvailablePlayers = playerListOptions;
            editProject.AvailableArtists = artistListOptions;
            editProject.Project = project;
            // These ids will show the players and artists that are already selected as highlighted in the edit view
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
        public async Task<IActionResult> Edit(int id, ProjectCreateEditViewModel editProject)
        {
            if (id != editProject.Project.ProjectId)
            {
                return NotFound();
            }

            ModelState.Remove("Project.User");
            ModelState.Remove("Project.UserId");
            if (ModelState.IsValid)
            {
                // Delete the joiner tables before the edit so that we can add the new instances without duplicates
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
                    // Add the new joiner tables to the database if there are any
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
                    // Update the database with the new edited project
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
                return RedirectToAction("ProjectsofStatus", new { id = 3 });
                // Add logic to get the dropdown items and multiselect items. Not Sure if I need this.
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

            // Get the single project from the database that has the passed in id as the ProjectId
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
            // Remove the joiner tables for both the player and artist. Foreach over them to remove each individual playerproject or artistproject
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
            // Remove the project from the database and save the changes
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            // Redirect to the projects only showing the projects with the status current
            return RedirectToAction("ProjectsofStatus", new { id = 3 });
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }
    }
}
