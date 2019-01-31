using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ClientCreateEditViewModel
    {
        public Client Client { get; set; }

        // A list to hold the Artists to choose from in the create and edit views
        public List<SelectListItem> AvailableArtists { get; set; }

        // A list to hold all the selected Artists Ids so that the joiner tables can be made and the chosen one can be highlighted for the edit view.
        public List<int> SelectedArtists { get; set; }
    }
}
