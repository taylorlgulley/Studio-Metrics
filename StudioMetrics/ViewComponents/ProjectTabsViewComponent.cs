using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioMetrics.Data;
using StudioMetrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.ViewComponents
{
    public class ProjectTabsViewModel
    {
        public string ActiveTab { get; set; }
        public int CountAll { get; set; } = 0;
        public int CountUpcoming { get; set; } = 0;
        public int CountCompleted { get; set; } = 0;
        public int CountCurrent { get; set; } = 0;
        public int CountTentative { get; set; } = 0;
    }

    public class ProjectTabsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectTabsViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string activeTab) 
        {
            // Get the current, authenticated user
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            ProjectTabsViewModel model = new ProjectTabsViewModel();

            model.ActiveTab = activeTab;

            var projects = await _context.Project
                .Where(p => p.User == user)
                .Include(p => p.StatusType)
                .ToListAsync();
                ;

            model.CountAll = projects.Count();
            model.CountUpcoming = projects.Where(p => p.StatusTypeId == 1).Count();
            model.CountCompleted = projects.Where(p => p.StatusTypeId == 2).Count();
            model.CountCurrent = projects.Where(p => p.StatusTypeId == 3).Count();
            model.CountTentative = projects.Where(p => p.StatusTypeId == 4).Count();

            return View(model);
        }

    }
}
