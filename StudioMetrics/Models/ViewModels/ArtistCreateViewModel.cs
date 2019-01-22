using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ArtistCreateViewModel
    {
        public Artist Artist { get; set; }

        // A list to hold the Clients to choose from in the create and edit views
        public List<SelectListItem> AvailableClients { get; set; }

        // A list to hold all the selected Clients Ids so that the joiner tables can be made and the chosen one can be highlighted for the edit view.
        public List<int> SelectedClients { get; set; }
    }
}
