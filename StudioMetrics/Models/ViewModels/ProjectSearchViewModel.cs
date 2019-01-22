using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ProjectSearchViewModel
    {
        // A string to hold the search term for the search view
        public string Search { get; set; }

        // A collection to hold each project retrieved from the database that has a title containing the search term
        public ICollection<Project> Projects { get; set; }
    }
}
