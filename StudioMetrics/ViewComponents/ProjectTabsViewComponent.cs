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
        /*have properties to count the number of projects in and pass it in to the view componet tabs you can pass it in like the active tab follow Steve's view component example*/
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
