using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models.ViewModels
{
    public class ClientEditViewModel
    {
        public Client Client { get; set; }

        public List<SelectListItem> AvailableArtists { get; set; }

        public List<int> SelectedArtists { get; set; }
    }
}
