using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ProjectSearchViewModel
    {

        public string Search { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
