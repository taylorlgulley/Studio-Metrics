using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ProjectCreateViewModel
    {
        public Project Project { get; set; }

        public List<SelectListItem> ProjectTypes { get; set; }

        public List<SelectListItem> StatusTypes { get; set; }

        public List<SelectListItem> Clients { get; set; }
    }
}
