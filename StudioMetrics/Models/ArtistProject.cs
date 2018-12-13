using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models
{
    public class ArtistProject
    {
        [Key]
        public int ArtistProjectId { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
