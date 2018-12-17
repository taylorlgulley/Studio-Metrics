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

        public List<SelectListItem> AvailableClients { get; set; }

        public List<int> SelectedClients { get; set; }
    }
}
