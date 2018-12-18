using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.ViewComponents
{
    public class ProjectTabsViewModel
    {
        public string ActiveTab { get; set; }
    }

    public class ProjectTabsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string activeTab) 
        {
            ProjectTabsViewModel model = new ProjectTabsViewModel();

            model.ActiveTab = activeTab;

            return View(model);
        }

    }
}
