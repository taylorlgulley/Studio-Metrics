using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ProjectCreateEditViewModel
    {
        public Project Project { get; set; }

        // Lists for ProjectTypes, StatusTypes, and Clients for dropdown options on the create and edit
        public List<SelectListItem> ProjectTypes { get; set; }

        public List<SelectListItem> StatusTypes { get; set; }

        public List<SelectListItem> Clients { get; set; }

        // Lists for both Players and Artists to have options as well as the options that were selected. The Selected are to create the joiner tables for the database and highlight those that have already been selected in the edit view
        public List<SelectListItem> AvailablePlayers { get; set; }

        public List<int> SelectedPlayers { get; set; }

        public List<SelectListItem> AvailableArtists { get; set; }

        public List<int> SelectedArtists { get; set; }
    }
}
